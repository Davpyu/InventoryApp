using DotNetService.Constants.Logger;
using DotNetService.Domain.Logging.Services;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Subscribtions;

namespace DotNetService.Domain.Logging.Listeners
{
    public class LoggingNATsListener(
        ILoggerFactory loggerFactory,
        LoggingService loggingService
    ) : ISubscribtionAction<IDictionary<string, object>>
    {

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        public readonly LoggingService _loggingService = loggingService;

        public void Handle(IDictionary<string, object> data)
        {
            var jsonData = Utils.JsonSerialize(data);
            _logger.LogInformation(jsonData);
        }
    }
}
