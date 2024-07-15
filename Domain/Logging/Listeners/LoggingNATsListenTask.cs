using DotNetService.Constants.Logger;
using DotNetService.Infrastructure.Shareds;
using NATS.Client.Core;
using DotNetService.Constants.Event;
using DotNetService.Infrastructure.Integrations.NATs;

namespace DotNetService.Domain.Logging.Listeners
{
    public class LoggingNATsListenTask(
        ILoggerFactory loggerFactory,
        NatsConnection natsConnection,
        IServiceScopeFactory serviceScopeFactory,
        NATsIntegration natsIntegration
        )
    {
        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);

        public void ListenAndReply()
        {
            string subject = natsIntegration.Subject(NATsEventModuleEnum.LOGGER, NATsEventActionEnum.DEBUG, NATsEventStatusEnum.INFO);

            _logger.LogInformation("Start Subscription With Reply of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsReplyService>();

                        var data = msg.Data;
                        var reply = action.Reply(Utils.JsonDeserialize<IDictionary<string, object>>(data));

                        var jsonReply = Utils.JsonSerialize(reply);
                        await msg.ReplyAsync(jsonReply, null, msg.ReplyTo);
                    }
                }
            );
        }

        public void ListenPost()
        {
            string subject = EndpointCallEventConstant.SUBS_POST_SUBJECT;

            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsService>();

                        var data = msg.Data;
                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                    }
                }
            );
        }

        public void ListenGet()
        {
            string subject = EndpointCallEventConstant.SUBS_GET_SUBJECT;

            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsService>();

                        var data = msg.Data;
                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                    }
                }
            );
        }

        public void ListenPut()
        {
            string subject = EndpointCallEventConstant.SUBS_PUT_SUBJECT;

            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsService>();

                        var data = msg.Data;
                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                    }
                }
            );
        }

        public void ListenPatch()
        {
            string subject = EndpointCallEventConstant.SUBS_PATCH_SUBJECT;

            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsService>();

                        var data = msg.Data;
                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                    }
                }
            );
        }

        public void ListenDelete()
        {
            string subject = EndpointCallEventConstant.SUBS_DELETE_SUBJECT;

            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsService>();

                        var data = msg.Data;
                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                    }
                }
            );
        }

        public void ListenOption()
        {
            string subject = EndpointCallEventConstant.SUBS_OPTIONS_SUBJECT;

            _logger.LogInformation("Start Subscription of {Subject} : ", subject);
            Task.Run(
                async () =>
                {
                    await foreach (var msg in natsConnection.SubscribeAsync<string>(subject))
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var action = scope.ServiceProvider.GetRequiredService<LoggingNATsService>();

                        var data = msg.Data;
                        action.Handle(Utils.JsonDeserialize<IDictionary<string, object>>(data));
                    }
                }
            );
        }
    }
}
