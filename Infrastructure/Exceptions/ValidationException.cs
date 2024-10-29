using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetService.Infrastructure.Exceptions
{
    public class ValidationException : Exception
    {
        public ModelStateDictionary ModelState { get; set; }

        public ValidationException(string message, ModelStateDictionary modelState) : base(message)
        {
            ModelState = modelState;
        }

    }
}