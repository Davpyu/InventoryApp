using DotNetService.Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;
using DotNetService.Constants.Logger;
using Quartz;

namespace DotNetService.Infrastructure.Jobs
{
    public class NotificationHouseKeepingJob(
        IamDBContext context,
        ILoggerFactory loggerFactory
    ) : IJob
    {
        private readonly IamDBContext _context = context;

        private readonly ILogger _logger = loggerFactory.CreateLogger(LoggerConstant.ACTIVITY);

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Notification housekeeping job started.");

            var notifications = await _context.Notifications
                .Where(n => n.CreatedAt < DateTime.Now.AddDays(-30))
                .ExecuteDeleteAsync();

            _logger.LogInformation("Notification housekeeping job completed. {NotificationsDeleted} notifications deleted.", notifications);
        }
    }
}