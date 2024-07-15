using DotNetService.Constants.Logger;
using DotNetService.Domain.Logging.Listeners;
using DotNetService.Infrastructure.Integrations.NATs;

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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("NATs Subscription Hosted Service running listen.");

                // === Logging === \\
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenPost();
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenGet();
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenPut();
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenPatch();
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenDelete();
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenOption();

                // === OtherModule === \\
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                _logger.LogInformation("NATs Subscription Hosted Service running reply.");

                // === Logging === \\
                scope.ServiceProvider.GetRequiredService<LoggingNATsListenTask>().ListenAndReply();

                // === OtherModule === \\
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NATs Subscription Hosted Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
