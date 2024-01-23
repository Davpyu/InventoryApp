using DotNetService.Constants.Logger;
using DotNetService.Domain.Logging.Services;
using DotNetService.Infrastructure.Shareds;
using DotNetService.Infrastructure.Subscribtions;
using Sentry;

namespace DotNetService.Domain.Logging.Listeners
{
    public class LoggingNATsListenAdnReply(
        ILoggerFactory loggerFactory,
        LoggingService loggingService
    ) : IReplyAction<IDictionary<string, object>, IDictionary<string, object>>
    {

        public readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.NATS);
        public readonly LoggingService _loggingService = loggingService;

        public IDictionary<string, object> Reply(IDictionary<string, object> data)
        {
            _logger.LogInformation("Request Data : " + Utils.JsonSerialize(data));
            var reply = new Dictionary<string, object> {
                { "status", "OK" },
            };

            _logger.LogInformation("Reply Data : " + Utils.JsonSerialize(reply));
            return reply;
        }
    }
}
