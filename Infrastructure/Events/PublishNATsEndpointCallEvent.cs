using DotNetService.Constants.Logger;
using DotNetService.Infrastructure.Integrations.NATs;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetService.Infrastructure.Events {
    public class PublishNATsEndpointCallEvent(
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
            await next();

            var subject = userId + "." + method.ToLower() + "." + endpoint;
            Utils.BackgroundProcessThreadAsync(async Task () => {
                try {
                    var reply = await _natsIntegration.PublishAndGetReply<string, object>(NATsEventConstant.SUBS_AND_REPLY_PREFIX_SUBJECT + "." + subject, Utils.JsonSerialize(context.ActionArguments));
                    _loggerIntegration.LogInformation("Publish NATs Event Reply with Subject : " + subject + " | Reply : " + reply);
                } catch (Exception err) {
                    _loggerIntegration.LogInformation("Publish NATs Event Error : " + subject);
                    _loggerIntegration.LogError(err.StackTrace);
                    Console.WriteLine("Reply Error");
                }
            });
        }
    }
}