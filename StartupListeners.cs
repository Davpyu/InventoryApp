using DotNetService.Domain.Inventory.Listeners;
using DotNetService.Domain.Logging.Listeners;

namespace DotNetService
{
    public partial class Startup
    {
        public void Listeners(IServiceCollection services)
        {
            services.AddScoped<LoggingNATsListener>();
            services.AddScoped<LoggingNATsListenAndReply>();
            services.AddScoped<CheckInventoryListenAndReply>();
        }
    }
}
