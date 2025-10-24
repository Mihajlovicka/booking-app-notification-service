using NotificationService.Model.Entity;

namespace NotificationService.Repository.Contract;

public interface INotificationUserRepository : ICrudRepository<NotificationUser>
{
    Task<IEnumerable<NotificationUser>> GetByUserIdAsync(string userId);
    Task DeleteByUserIdAsync(string userId);
    Task AddManyAsync(IEnumerable<NotificationUser> users);
    Task<NotificationUser?> GetByUserAndTypeAsync(string userId, int notificationTypeId);
}