using Microsoft.AspNetCore.Authorization;
using DotNetService.Domain.Auth.Util;
using DotNetService.Infrastructure.Attributes;
using DotNetService.Domain.Permission.Util;
using DotNetService.Infrastructure.Databases;
using Microsoft.AspNetCore.Authentication;
using DotNetService.Models;

namespace DotNetService.Infrastructure.Middlewares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly LocalStorageDatabase _localStorage;

        public AuthorizationMiddleware(
            RequestDelegate next,
            LocalStorageDatabase localStorage,
            IConfiguration config
        )
        {
            _next = next;
            _localStorage = localStorage;
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
            var userId = context.User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value;

            var localStorageKey = AuthUtility.GenerateKeyLocalStorage(userId);
            
            var userAuthInfo = await _localStorage.Get<UserAuthInfo>(localStorageKey);

            if (userAuthInfo == null)
            {
                context.Response.StatusCode = 401;
                await context.SignOutAsync();
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            if (endpoint?.Metadata?.GetMetadata<PermissionsAttribute>() is PermissionsAttribute permissionAttr)
            {
                // Extract required permissions from the attribute
                var requiredPermissions = permissionAttr.Permissions;
                var userPermissions = userAuthInfo.Permissions;

                // Validate that the user has the required permissions
                PermissionUtil.ValidatePermission(userPermissions.ToArray(), requiredPermissions);
            }

            await _next(context);
        }
    }
}

