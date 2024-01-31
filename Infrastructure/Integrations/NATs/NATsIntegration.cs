
using DotNetService.Constants.Logger;
using DotNetService.Exceptions;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Subscribtions;
using NATS.Client.Core;
using NATS.Client;

namespace DotNetService.Infrastructure.Integrations.NATs 
{
    public class NATsIntegration(
        ILoggerFactory loggerFactory,
        IConfiguration config,
        NatsConnection natsConnection
        )
    {
        private readonly IConfiguration _config = config;
        private readonly NatsConnection _natsConnection = natsConnection;
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);

        public void Subs<T>(string subject) {
            var task = Task.Run(
                async () => {
                    await foreach (var msg in _natsConnection.SubscribeAsync<T>(subject))
                    {
                        Console.WriteLine($"Received {msg.Subject}: {msg.Data}\n");
                    }
                }
            );
        }

        public void Subs<T>(string subject, ISubscribtionActionAsync<T> subAction) {
            var task = Task.Run(
                async () => {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        var data = msg.Data;
                        await subAction.HandleAsync(Utils.JsonDeserialize<T>(data));
                    }
                }
            );
        }

        public void SubsAndReply<T, R>(string subject, IReplyAsyncAction<T, R> subAction) {
            var task = Task.Run(
                async () => {
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

        public void SubsAndReply<T, R>(string subject, IReplyAction<T, R> subAction) {
            var task = Task.Run(
                async () => {
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

        public void Subs<T>(string subject, ISubscribtionAction<T> subAction) {
            var task = Task.Run(
                async () => {
                    await foreach (var msg in _natsConnection.SubscribeAsync<string>(subject))
                    {
                        var data = msg.Data;
                        subAction.Handle(Utils.JsonDeserialize<T>(data));
                    }
                }
            );
        }


        public async Task UnSub<T>(INatsSub<T> sub) {
            await sub.UnsubscribeAsync();
        }

        public async Task Publish<T>(string subject, T data) {
            await _natsConnection.PublishAsync(subject, data);
        }

        public async Task<R> PublishAndGetReply<T, R>(string subject, T data) {
            try
            {
                var msg = await _natsConnection.RequestAsync<T, string>(subject, data);
                var repliedData = msg.Data;

                return Utils.JsonDeserialize<R>(repliedData);
            }
            catch (NatsException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw new ServiceUnavailableException();
            }
        }
    }
}
