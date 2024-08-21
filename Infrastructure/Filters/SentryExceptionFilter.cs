using DotNetService.Exceptions;
using Sentry.Extensibility;

namespace DotNetService.Infrastructure.Filters
{
    public class SentryExceptionFilter : IExceptionFilter
    {
        private static List<Type> FilteredExceptionTypes()
        {
            // Write your custom exception that should be filtered here
            return [
                typeof(FluentValidation.ValidationException),
                typeof(BadHttpRequestException),
                typeof(DataNotFoundException),
                typeof(ValidationException),
                typeof(NotAllowedException),
                typeof(UnauthenticatedException),
                typeof(UnprocessableEntityException),
            ];
        }

        public bool Filter(Exception ex)
        {
            if (FilteredExceptionTypes().Contains(ex.GetType())) return true;

            return false;
        }
    }
}