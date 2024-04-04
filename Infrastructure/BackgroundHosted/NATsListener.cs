
using DotNetService.Constants.Event;
using DotNetService.Constants.Logger;
using DotNetService.Domain.Logging.Listeners;
using DotNetService.Infrastructure.Integrations.NATs;
using DotNetService.Infrastructure.Subscriptions;

namespace DotNetService.Infrastructure.BackgroundHosted
{
    public class NATsListener(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        NATsIntegration natsIntegration
    ) : IHostedService, IDisposable
    {

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);
        public readonly NATsIntegration _natsIntegration = natsIntegration;
        public readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

        public void Dispose()
        {
            // TODO: Dispose
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NATs Subscription Hosted Service running listen.");
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var loggingNatsListener = scope.ServiceProvider.GetRequiredService<LoggingNATsListener>();

                var listeners = new Dictionary<string, ISubscriptionAction<IDictionary<string, object>>>
                {
                    // Define all listeners here
                    { EndpointCallEventConstant.SUBS_POST_SUBJECT,  loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_GET_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_PUT_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_PATCH_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_DELETE_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_OPTIONS_SUBJECT, loggingNatsListener},
                };

                foreach (var listener in listeners)
                {
                    _natsIntegration.Subs(listener.Key, listener.Value);
                }
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var loggingNATsListenAndReply = scope.ServiceProvider.GetRequiredService<LoggingNATsListenAndReply>();

                var listenersAndReply = new Dictionary<string, IReplyAction<IDictionary<string, object>, IDictionary<string, object>>>
                {
                    // Logger
                    {
                        _natsIntegration.Subject(NATsEventModuleEnum.LOGGER, NATsEventActionEnum.DEBUG, NATsEventStatusEnum.INFO),
                        loggingNATsListenAndReply
                    }
                };

                _logger.LogInformation("NATs Subscription Hosted Service running reply.");
                foreach (var listener in listenersAndReply)
                {
                    _natsIntegration.SubsAndReply(listener.Key, listener.Value);
                }
            }
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NATs Subscription Hosted Service is stopping.");
        }
    }
}
