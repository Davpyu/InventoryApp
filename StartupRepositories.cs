using DotNetService.Domain.Permission.Repositories;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Domain.RolePermission.Repositories;
using DotNetService.Domain.User.Repositories;
using DotNetService.Domain.UserRole.Repositories;

namespace DotNetService
{
    public partial class Startup
    {
        public void Repositories(IServiceCollection services)
        {
            services.AddScoped<UserQueryRepository>();
            services.AddScoped<UserStoreRepository>();
            services.AddScoped<RoleQueryRepository>();
            services.AddScoped<RoleStoreRepository>();
            services.AddScoped<RolePermissionQueryRepository>();
            services.AddScoped<RolePermissionStoreRepository>();
            services.AddScoped<PermissionQueryRepository>();
            services.AddScoped<PermissionStoreRepository>();
            services.AddScoped<UserRoleQueryRepository>();
            services.AddScoped<UserRoleStoreRepository>();
        }
    }
}
