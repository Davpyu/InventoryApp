using DotNetService.Constants.Logger;
using DotNetService.Exceptions;
using DotNetService.Infrastructure.Shareds;
using NATS.Client.Core;
using DotNetService.Constants.Event;
using DotNetService.Infrastructure.Subscriptions;

namespace DotNetService.Infrastructure.Integrations.NATs
{
    public class NATsIntegration(
        ILoggerFactory loggerFactory,
        NatsConnection natsConnection
        )
    {
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);

        public string Subject(
            NATsEventModuleEnum modul,
            NATsEventActionEnum action,
            NATsEventStatusEnum status
        )
        {
            string subject = $"{modul}.{action}.{status}";

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
            _logger.LogInformation("Publish With Subject : {Subject} | Data : {Data}", subject, data);
            await natsConnection.PublishAsync(subject, data);
        }

        public async Task<R> PublishAndGetReply<T, R>(string subject, T data)
        {
            _logger.LogInformation("Publish With Subject : {Subject} | Data : {Data}", subject, data);
            try
            {
                var msg = await natsConnection.RequestAsync<T, string>(subject, data);
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
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
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

        public void InitListenAndReplyTask<TListenAndReply>(IServiceScopeFactory serviceScopeFactory, string subject) where TListenAndReply : IReplyAction<IDictionary<string, object>, IDictionary<string, object>>
        {
            _logger.LogInformation("Start Subscription With Reply Of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
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
    }
}
