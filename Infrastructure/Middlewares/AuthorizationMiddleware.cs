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

            // Validate user permissions against the required permissions for the endpoint
            if (endpoint?.Metadata?.GetMetadata<PermissionsAttribute>() is PermissionsAttribute permissionAttr)
            {
                // Extract required permissions from the attribute
                var requiredPermissions = permissionAttr.Permissions;

                // Retrieve the permissions claim from the user context
                var permissionsClaim = context.User.Claims.FirstOrDefault(claim => claim.Type == "permissions")?.Value;

                // Deserialize permissions or use an empty array if none are provided
                var userPermissions = string.IsNullOrEmpty(permissionsClaim)
                    ? Array.Empty<string>()
                    : JsonSerializer.Deserialize<string[]>(permissionsClaim) ?? Array.Empty<string>();

                // Validate that the user has the required permissions
                PermissionUtil.ValidatePermission(userPermissions, requiredPermissions);
            }


            await _next(context);
        }
    }
}

