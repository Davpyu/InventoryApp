using DotNetService.Domain.Auth.Services;
using DotNetService.Domain.Logging.Services;
using DotNetService.Domain.Permission.Services;
using DotNetService.Domain.Role.Services;
using DotNetService.Domain.User.Services;
using DotNetService.Infrastructure.Databases;

namespace DotNetService
{
    public partial class Startup
    {
        public void Services(IServiceCollection services)
        {
            services.AddSingleton<LocalStorageDatabase>();

            services.AddScoped<AuthService>();
            services.AddScoped<UserService>();
            services.AddScoped<PermissionService>();
            services.AddScoped<RoleService>();
            services.AddScoped<LoggingService>();
        }
    }
}
