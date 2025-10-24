using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Model.Entity;
using NotificationService.Repository.Contract;

namespace NotificationService.Repository.Implementation;

public class NotificationUserRepository : CrudRepository<NotificationUser>, INotificationUserRepository
{
    public NotificationUserRepository(AppDbContext context) : base(context, "NotificationUser") { }

    public async Task<IEnumerable<NotificationUser>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<NotificationUser>.Filter.Eq(nu => nu.UserExternalId, userId);
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task DeleteByUserIdAsync(string userId)
    {
        var filter = Builders<NotificationUser>.Filter.Eq(nu => nu.UserExternalId, userId);
        await _collection.DeleteManyAsync(filter);
    }

    public async Task AddManyAsync(IEnumerable<NotificationUser> users)
    {
        if (users.Any())
            await _collection.InsertManyAsync(users);
    }

    public async Task<NotificationUser?> GetByUserAndTypeAsync(string userId, int notificationTypeId)
    {
        var filter = Builders<NotificationUser>.Filter.And(
            Builders<NotificationUser>.Filter.Eq(u => u.UserExternalId, userId),
            Builders<NotificationUser>.Filter.Eq(u => u.NotificationTypeId, notificationTypeId)
        );
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }
}