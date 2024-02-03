
using DotNetService.Constants.Logger;
using DotNetService.Domain.Logging.Listeners;
using DotNetService.Infrastructure.Integrations.NATs;
using DotNetService.Infrastructure.Subscribtions;

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
                var listeners = new Dictionary<string, ISubscribtionAction<IDictionary<string, object>>>
                {
                    // Define all listeners here
                    { EndpointCallEventConstant.SUBS_POST_SUBJECT,  loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_GET_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_PUT_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_PATCH_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_DELETE_SUBJECT, loggingNatsListener},
                    { EndpointCallEventConstant.SUBS_OPTIONS_SUBJECT, loggingNatsListener}
                };
                
                foreach (var listener in listeners) {
                    _natsIntegration.Subs(listener.Key, listener.Value); 
                }
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {   
                var listenersAndReply = new Dictionary<string, IReplyAction<IDictionary<string, object>, IDictionary<string, object>>>
                {
                    // Define all listenersAndReply here
                    { NATsEventConstant.SUBS_AND_REPLY_ALL, scope.ServiceProvider.GetRequiredService<LoggingNATsListenAndReply>() }
                };
                
                _logger.LogInformation("NATs Subscription Hosted Service running reply.");
                foreach (var listener in listenersAndReply) {
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
