using NotificationService.Data.Dto;
using NotificationService.Model.Dto;

namespace NotificationService.Service.Contract;

public interface INotificationService
{
    Task<IEnumerable<NotificationTypeDto>> GetAllByRole();

    Task<IEnumerable<NotificationTypeDto>> GetAllByCurrentUser();

    Task<IEnumerable<NotificationTypeDto>> Save(NotificationTypeDto[] notifications);

    Task<IEnumerable<NotificationDto>> GetOldNotifications();
    Task<IEnumerable<NotificationDto>> UpdateOldNotifications(IEnumerable<NotificationDto> notificationDtos);
}