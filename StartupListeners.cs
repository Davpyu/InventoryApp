using DotNetService.Domain.Logging.Listeners;

namespace DotNetService
{
    public partial class Startup
    {
        public void Listeners(IServiceCollection services)
        {
            services.AddScoped<LoggingNATsListenAndReply>();
            services.AddScoped<LoggingNATsListenAndReply>();
        }
    }
}
