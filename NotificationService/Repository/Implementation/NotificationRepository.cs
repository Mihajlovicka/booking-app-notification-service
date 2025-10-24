using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Model.Entity;
using NotificationService.Repository.Contract;

namespace NotificationService.Repository.Implementation;


public class NotificationRepository : CrudRepository<Notification>, INotificationRepository
{
    public NotificationRepository(AppDbContext context) : base(context, "Notification") { }

    public async Task<IEnumerable<Notification>> GetByNotificationUserIdAsync(string notificationUserId)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.NotificationUserExternalId, notificationUserId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task DeleteByUserExternalIdAsync(string userExternalId)
    {
        var filter = Builders<Notification>.Filter.Eq(n => n.NotificationUserExternalId, userExternalId);
        await _collection.DeleteManyAsync(filter);
    }

    public async Task AddManyAsync(IEnumerable<Notification> notificationUsers)
    {
        if (notificationUsers == null || !notificationUsers.Any())
            return;

        await _collection.InsertManyAsync(notificationUsers);
    }
}