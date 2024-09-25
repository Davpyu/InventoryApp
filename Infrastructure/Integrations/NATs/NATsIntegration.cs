using DotNetService.Constants.Logger;
using DotNetService.Exceptions;
using DotNetService.Infrastructure.Shareds;
using NATS.Client.Core;
using DotNetService.Constants.Event;
using DotNetService.Infrastructure.Subscriptions;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace DotNetService.Infrastructure.Integrations.NATs
{
    public class NATsIntegration
    {
        public readonly ILogger _logger;
        public readonly INatsConnection _natsConnection;
        public readonly INatsJSContext _js;

        public NATsIntegration(
            ILoggerFactory loggerFactory,
            NatsConnection natsConnection
        ) {
            _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
            _js = new NatsJSContext(natsConnection);
            _natsConnection = natsConnection;
        }

        public string Subject(
            NATsEventModuleEnum modul,
            NATsEventActionEnum action,
            NATsEventStatusEnum status,
            NATsEventNATSTypeEnum type = NATsEventNATSTypeEnum.CORE
        )
        {
            string subject = $"{type}.{modul}.{action}.{status}";

            subject = subject.Replace(NATsEventCommon.ALL.ToString(), "*");

            return subject.ToLower();
        }

        public async Task UnSub<T>(INatsSub<T> sub)
        {
            _logger.LogInformation("Stop Subscription Of Subject : {Subject}", sub.Subject);
            await sub.UnsubscribeAsync();
        }

        public async Task Publish<T>(string subject, T data)
        {
            _logger.LogError("Start Publishing with subject : {subject} | data : {data}", subject, data);
            try
            {
                var ack = await _js.PublishAsync(subject, data);
                ack.EnsureSuccess();
            }
            catch (NatsJSException e)
            {
                _logger.LogError("Error StackTrace: {StackTrace}", e.StackTrace);
                throw new ServiceUnavailableException();
            }
        }

        public async Task<R> PublishAndGetReply<T, R>(string subject, T data)
        {
            _logger.LogInformation("Publish With Subject : {Subject} | Data : {Data}", subject, data);
            try
            {
                var msg = await _natsConnection.RequestAsync<T, string>(subject, data);
                var repliedData = msg.Data;

                _logger.LogInformation("Get Reply With Subject : {Subject} | Reply : {Reply}", subject, repliedData);
                return Utils.JsonDeserialize<R>(repliedData);
            }
            catch (NatsException e)
            {
                _logger.LogError("Error StackTrace: {StackTrace}", e.StackTrace);
                throw new ServiceUnavailableException();
            }
        }
        
        public void InitListenTask<TListen>(IServiceScopeFactory serviceScopeFactory, string subject) where TListen : ISubscriptionAction<IDictionary<string, object>>
        {
            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        try
                        {
                            _logger.LogInformation("Get Subscribed Event {subject} | Data {msg.Data}", subject, msg.Data);

                            using var scope = serviceScopeFactory.CreateScope();
                            var action = scope.ServiceProvider.GetRequiredService<TListen>();

                            var data = msg.Data;
                            action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error On Get Subscribed {Event} | Data {msg.Data}", subject, msg.Data);
                        }
                    }
                }
            );
        }

        public async Task InitAsyncListenAndReplyTask<TListenAndReply>(IServiceScopeFactory serviceScopeFactory, string subject) where TListenAndReply : IReplyAsyncAction<IDictionary<string, object>, IDictionary<string, object>>
        {
            _logger.LogInformation("Start Subscription With Async Reply Of {Subject} : ", subject);
            await Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        try
                        {
                            _logger.LogInformation("Get Subscribed Event {subject} | Data {msg.Data}", subject, msg.Data);
                            using var scope = serviceScopeFactory.CreateScope();
                            var action = scope.ServiceProvider.GetRequiredService<TListenAndReply>();
                            var data = msg.Data;
                            var reply = await action.ReplyAsync(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                            var jsonReply = Utils.JsonSerialize(reply);
                            await msg.ReplyAsync(jsonReply, null, msg.ReplyTo);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error On Get Subscribed {Event} | Data {msg.Data}", subject, msg.Data);
                        }
                    }
                }
            );
        }

        public void InitListenAndReplyTask<TListenAndReply>(IServiceScopeFactory serviceScopeFactory, string subject) where TListenAndReply : IReplyAction<IDictionary<string, object>, IDictionary<string, object>>
        {
            _logger.LogInformation("Start Subscription With Reply Of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        try
                        {
                            _logger.LogInformation("Get Subscribed Event {subject} | Data {msg.Data}", subject, msg.Data);

                            using var scope = serviceScopeFactory.CreateScope();
                            var action = scope.ServiceProvider.GetRequiredService<TListenAndReply>();

                            var data = msg.Data;
                            var reply = action.Reply(Utils.JsonDeserialize<IDictionary<string, object>>(data));

                            var jsonReply = Utils.JsonSerialize(reply);
                            await msg.ReplyAsync(jsonReply, null, msg.ReplyTo);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error On Get Subscribed {Event} | Data {msg.Data}", subject, msg.Data);
                        }
                    }
                }
            );
        }
 
        public async Task InitPushListenerTask<TListen>(IServiceScopeFactory serviceScopeFactory, string streamName, string subject) where TListen : ISubscriptionAction<IDictionary<string, object>>
        {
            _logger.LogInformation("Start Subscription NATs JetStream of {Subject} : ", subject);
            await Task.Run(
                async () =>
                {
                    // Create a consumer on a stream to receive the messages
                    var consumerName = (streamName + "_consumer_push_processor").ToLower();

                    var consumerConfig = new ConsumerConfig(consumerName) {
                        DeliverPolicy = ConsumerConfigDeliverPolicy.New,
                        FilterSubject = subject,
                        AckPolicy = ConsumerConfigAckPolicy.Explicit,
                        MaxAckPending = 1,
                        AckWait = TimeSpan.FromSeconds(10),
                        MaxDeliver = 1
                    };

                    var consumer = await _js.CreateOrUpdateConsumerAsync(streamName, consumerConfig);

                    await foreach (var jsMsg in consumer.ConsumeAsync<string>())
                    {
                        
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<TListen>();
                        var data = jsMsg.Data;

                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));

                        await jsMsg.AckAsync();
                    }
                }
            );
        }

        public async Task InitPullListenerTask<TListen>(IServiceScopeFactory serviceScopeFactory, string streamName, string subject, int maxConsumption = 10) where TListen : ISubscriptionAction<IDictionary<string, object>>
        {
            await Task.Run(
                async () =>
                {
                    var consumerName = (streamName + "_consumer_pull_processor").ToLower();

                    var consumerConfig = new ConsumerConfig(consumerName) {
                        DeliverPolicy = ConsumerConfigDeliverPolicy.All,
                        FilterSubject = subject,
                        MaxAckPending = 0,
                        AckWait = TimeSpan.FromSeconds(5),
                        MaxDeliver = -1,
                    };

                    var consumer = await _js.CreateOrUpdateConsumerAsync(streamName, consumerConfig);

                    while(true) {
                        await foreach (var jsMsg in consumer.FetchAsync<string>(opts: new NatsJSFetchOpts { MaxMsgs = maxConsumption }))
                        {
                            using var scope = serviceScopeFactory.CreateScope();
                            var action = scope.ServiceProvider.GetRequiredService<TListen>();
                            var data = jsMsg.Data;

                            action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));

                            await jsMsg.AckAsync();
                        }

                        await Task.Delay(1000);
                        _logger.LogInformation("Slept for 1 second");
                    }
                }
            );
        }
    }
}
