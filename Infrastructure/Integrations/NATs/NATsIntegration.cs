
using DotNetService.Constants.Logger;
using DotNetService.Exceptions;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Subscriptions;
using NATS.Client.Core;
using NATS.Client;
using DotNetService.Constants.Event;

namespace DotNetService.Infrastructure.Integrations.NATs
{
    public class NATsIntegration(
        ILoggerFactory loggerFactory,
        NatsConnection natsConnection
        )
    {
        private readonly NatsConnection _natsConnection = natsConnection;
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);

        public string Subject(
            NATsEventModuleEnum modul,
            NATsEventActionEnum action,
            NATsEventStatusEnum status
        )
        {
            string subject = modul + "." + action + "." + status;
            
            subject = subject.Replace(NATsEventCommonEnum.ALL.ToString(), ">");

            return subject.ToLower();
        }

        public void Subs<T>(string subject, ISubscriptionActionAsync<T> subAction)
        {
            _logger.LogInformation("Start Subscription Of Subject : " + subject);
            var task = Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        var data = msg.Data;
                        await subAction.HandleAsync(Utils.JsonDeserialize<T>(data));
                    }
                }
            );
        }

        public void Subs<T>(string subject, ISubscriptionAction<T> subAction)
        {
            _logger.LogInformation("Start Subscription Of Subject : " + subject);
            var task = Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        var data = msg.Data;
                        subAction.Handle(Utils.JsonDeserialize<T>(data));
                    }
                }
            );
        }

        public void SubsAndReply<T, R>(string subject, IReplyAsyncAction<T, R> subAction)
        {
            _logger.LogInformation("Start Subscription With Reply Of Subject : " + subject);
            var task = Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        var data = msg.Data;
                        var reply = await subAction.ReplyAsync(Utils.JsonDeserialize<T>(data));

                        var jsonReply = Utils.JsonSerialize(reply);
                        await msg.ReplyAsync(jsonReply, null, msg.ReplyTo);
                    }
                }
            );
        }

        public void SubsAndReply<T, R>(string subject, IReplyAction<T, R> subAction)
        {
            _logger.LogInformation("Start Subscription With Reply Of Subject : " + subject);
            var task = Task.Run(
                async () =>
                {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        var data = msg.Data;
                        var reply = subAction.Reply(Utils.JsonDeserialize<T>(data));

                        var jsonReply = Utils.JsonSerialize(reply);
                        await msg.ReplyAsync(jsonReply, null, msg.ReplyTo);
                    }
                }
            );
        }

        public async Task UnSub<T>(INatsSub<T> sub)
        {
            _logger.LogInformation("Stop Subscription Of Subject : " + sub.Subject);
            await sub.UnsubscribeAsync();
        }

        public async Task Publish<T>(string subject, T data)
        {
            _logger.LogInformation("Publish With Subject : " + subject + " | Data : " + data);
            await _natsConnection.PublishAsync(subject, data);
        }

        public async Task<R> PublishAndGetReply<T, R>(string subject, T data)
        {
            _logger.LogInformation("Publish With Subject : " + subject + " | Data : " + data);
            try
            {
                var msg = await _natsConnection.RequestAsync<T, string>(subject, data);
                var repliedData = msg.Data;

                _logger.LogInformation("Get Reply With Subject : " + subject + " | Reply : " + data);
                return Utils.JsonDeserialize<R>(repliedData);
            }
            catch (NatsException e)
            {
                _logger.LogError(e.StackTrace);
                throw new ServiceUnavailableException();
            }
        }
    }
}
