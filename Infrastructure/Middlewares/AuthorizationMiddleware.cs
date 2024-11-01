using Microsoft.AspNetCore.Authorization;
using DotNetService.Domain.Auth.Util;
using DotNetService.Infrastructure.Attributes;
using System.Text.Json;
using DotNetService.Domain.Permission.Util;

namespace DotNetService.Infrastructure.Middlewares
{
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
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
            {
                await _next(context);
                return;
            }

            var token = string.Empty;
            var headers = context.Request.Headers;
            if (headers.ContainsKey("Authorization") && headers.Authorization.ToString().StartsWith("Bearer "))
            {
                token = headers.Authorization.ToString().Replace("Bearer ", string.Empty);
            }

            AuthUtility.ValidateJwtToken(_config["JWTSetting:Secret"], token);

            var user = AuthUtility.GetUserLogged(token);

            context.User = AuthUtility.ClaimPrincipalWithJson(user);

            // Validate Permissions
            if (endpoint?.Metadata?.GetMetadata<PermissionsAttribute>() is object)
            {
                var requiredPermissions = endpoint.Metadata.GetMetadata<PermissionsAttribute>()?.Permissions;
                var permissionsString = context.User.Claims.FirstOrDefault(claim => string.Equals(claim.Type, "permissions"))?.Value;
                var permissions = string.IsNullOrEmpty(permissionsString)
                    ? [] : JsonSerializer.Deserialize<string[]>(permissionsString);
                PermissionUtil.ValidatePermission(permissions, requiredPermissions);
            }

            await _next(context);
        }
    }
}

