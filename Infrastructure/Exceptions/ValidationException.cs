using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DotNetService.Exceptions
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