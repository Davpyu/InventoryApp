using DotNetService.Infrastructure.Exceptions;
using DotNetService.Infrastructure.Dtos;
using DotNetService.Domain.Notification.Repositories;
using DotNetService.Domain.Notification.Dtos;
using DotNetService.Domain.Notification.Messages;

namespace DotNetService.Domain.Notification.Services
{
    public class NotificationService(
        NotificationQueryRepository NotificationQueryRepository
    )
    {
        private readonly NotificationQueryRepository _notificationQueryRepository = NotificationQueryRepository;

        public async Task<PaginationModel<NotificationResultDto>> Index(NotificationQueryDto query, Guid userId)
        {
            var result = await _notificationQueryRepository.Pagination(query, userId);
            var formattedResult = NotificationResultDto.MapRepo(result.Data);
            var paginate = PaginationModel<NotificationResultDto>.Parse(formattedResult, result.Count, query);
            return paginate;
        }

        public async Task<NotificationResultDto> DetailById(Guid id, Guid userId)
        {
            var notification = await _notificationQueryRepository.FindOneByIdAndUserId(id, userId);

            if (notification == null)
            {
                throw new DataNotFoundException(NotificationErrorMessage.ErrNotificationNotFound);
            }

            return new NotificationResultDto(notification);
        }

        public async Task<bool> HasUnreadNotificationByUserId(Guid userId)
        {
            return await _notificationQueryRepository.HasUnreadNotificationByUserId(userId);
        }
    }
}