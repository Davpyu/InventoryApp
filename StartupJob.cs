using DotNetService.Domain.Auth.Repositories;
using DotNetService.Domain.Notification.Repositories;
using DotNetService.Domain.Permission.Repositories;
using DotNetService.Domain.Role.Repositories;
using DotNetService.Domain.User.Repositories;
using DotNetService.Infrastructure.Jobs;

namespace DotNetService
{
    public partial class Startup
    {
        public void Jobs(IServiceCollection services)
        {
            services.AddScoped<NotificationHouseKeepingJob>();
        }
    }
}
