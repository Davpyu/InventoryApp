using DotNetService.Constants.Event;
using DotNetService.Constants.Logger;
using DotNetService.Infrastructure.Integrations.NATs;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetService.Infrastructure.Events
{
    public class PublishNATsLoggingEvent(
        NATsIntegration natsIntegration,
        ILoggerFactory loggerFactory
    ) : IAsyncActionFilter
    {
        public readonly ILogger _loggerIntegration = loggerFactory.CreateLogger(LoggerConstant.INTEGRATION);
        public readonly NATsIntegration _natsIntegration = natsIntegration;

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            var userId = isAuthenticated ? context.HttpContext.User.FindFirst("id")?.Value.ToString() : Guid.Empty.ToString();
            var endpoint = context.HttpContext.Request.Path;
            var dataTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            string subject = _natsIntegration.Subject(NATsEventModuleEnum.LOGGER, NATsEventActionEnum.DEBUG, NATsEventStatusEnum.INFO, NATsEventNATSTypeEnum.JETSTREAM);

            await next();

            Utils.BackgroundProcessThreadAsync(async Task () =>
            {
                {
                    var data = new
                    {
                        dataTime,
                        userId,
                        method,
                        endpoint,
                        actionArguments = context.ActionArguments,
                    };

                    await _natsIntegration.Publish<object>(subject, Utils.JsonSerialize(data));
                }
            });
        }
    }
}