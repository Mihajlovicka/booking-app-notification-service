using NotificationService.Model.Entity;

namespace NotificationService.Repository.Contract;

public interface INotificationRepository : ICrudRepository<Notification> 
{
    Task<IEnumerable<Notification>> GetByNotificationUserIdAsync(string userExternalId);
    Task DeleteByUserExternalIdAsync(string userExternalId);
    Task AddManyAsync(IEnumerable<Notification> notificationUsers);
}