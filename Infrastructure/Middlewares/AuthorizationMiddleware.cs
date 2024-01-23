using System;
using System.Security.Claims;
using DotNetService.Exceptions;

namespace DotNetService.Infrastructure.Middlewares {
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public AuthorizationMiddleware(
            RequestDelegate next,
            IConfiguration config
        )
        {
            _next = next;
            _config = config;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = string.Empty;
            var headers = context.Request.Headers;
            if (headers.ContainsKey("Authorization") && headers.Authorization.ToString().StartsWith("Bearer "))
            {
                token = headers.Authorization.ToString().Replace("Bearer ", "");
            }
            
            var id = AuthUtility.ValidateJwtTokenAndGetId(_config["App:DataProtectionKey"], token);

            context.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { 
                        new (ClaimTypes.PrimarySid, id.ToString()) 
                    }
                )
            );

            // Panggil middleware selanjutnya dalam pipeline
            await _next(context);
        }
    }
}

