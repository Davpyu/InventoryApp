using DotNetService.Constants.Logger;
using DotNetService.Infrastructure.Integrations.NATs;
using DotNetService.Infrastructure.Shareds;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetService.Infrastructure.Events {
    public class PublishNATsEndpointCallEvent(
        NATsIntegration natsIntegration
    ) : IAsyncActionFilter
    {
        public readonly NATsIntegration _natsIntegration = natsIntegration;

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var endpoint = context.HttpContext.Request.Path;
            await next();

            var subject = method.ToLower() + "." + endpoint;
            // await _natsIntegration.Publish(subject, Utils.JsonSerialize(context.ActionArguments));
            Console.WriteLine(NATsEventConstant.SUBS_AND_REPLY_PREFIX_SUBJECT + "." + subject);
            var reply = await _natsIntegration.PublishAndGetReply<string, object>(NATsEventConstant.SUBS_AND_REPLY_PREFIX_SUBJECT + "." + subject, Utils.JsonSerialize(context.ActionArguments));
            Console.WriteLine(reply);
        }
    }
}