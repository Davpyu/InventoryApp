using System.Data.Entity.Core.Objects;
using System.Net;
using Amazon.Runtime.Internal;
using DotNetService.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotNetService.Infrastructure.Filters {
    public class ValidatorAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                throw new ValidationException("Validation Error", context.ModelState);
            }

            base.OnActionExecuting(context);
        }
    }
}
