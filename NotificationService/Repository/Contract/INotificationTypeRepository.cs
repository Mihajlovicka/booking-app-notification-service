using NotificationService.Model.Entity;

namespace NotificationService.Repository.Contract;

public interface INotificationTypeRepository : ICrudRepository<NotificationType>
{
    Task<List<NotificationType>> GetByRoleAsync(Role role);
    Task<List<NotificationType>> GetByIdsAsync(IEnumerable<int> ids);
}


