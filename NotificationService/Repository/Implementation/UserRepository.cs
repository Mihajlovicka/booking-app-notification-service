using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Model.Entity;
using NotificationService.Repository.Contract;

namespace NotificationService.Repository.Implementation;

public class UserRepository : CrudRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context, "User") { }

    public async Task<User?> GetByExternalIdAsync(Guid externalId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.ExternalId, externalId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Username, username);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public override async Task DeleteAllForUserAsync(string id)
    {
        // Cascade delete logic here (as defined earlier)
        var notifUserCollection = _collection.Database.GetCollection<NotificationUser>("NotificationUser");
        var notifCollection = _collection.Database.GetCollection<Notification>("Notification");

        var notifUsers = await notifUserCollection.Find(nu => nu.UserExternalId == id).ToListAsync();

        var notifUserIds = notifUsers.Select(nu => nu.Id).ToList();
        if (notifUserIds.Count > 0)
        {
            var notifFilter = Builders<Notification>.Filter.In(n => n.NotificationUserExternalId, notifUserIds);
            await notifCollection.DeleteManyAsync(notifFilter);
        }

        var notifUserFilter = Builders<NotificationUser>.Filter.Eq(nu => nu.UserExternalId, id);
        await notifUserCollection.DeleteManyAsync(notifUserFilter);

        await base.DeleteAllForUserAsync(id);
    }
}