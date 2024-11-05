using DotNetService.Applications.RolePermission.Service;
using DotNetService.Domain.Auth.Services;
using DotNetService.Domain.Logging.Services;
using DotNetService.Domain.Permission.Services;
using DotNetService.Domain.Role.Services;
using DotNetService.Domain.User.Services;

namespace DotNetService
{
    public partial class Startup
    {
        public void Services(IServiceCollection services)
        {
            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<RolePermissionService>();
            services.AddScoped<RoleService>();

            services.AddScoped<LoggingService>();
        }
    }
}
