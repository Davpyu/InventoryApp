using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System;

namespace DotNetService.Exceptions
{
    public class UnauthenticatedException : Exception
    {

        public UnauthenticatedException() : base("Token not valid")
        {
            // message can use in here
        }

    }
}