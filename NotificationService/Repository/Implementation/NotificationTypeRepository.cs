using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Model.Entity;
using NotificationService.Repository.Contract;

namespace NotificationService.Repository.Implementation;

public class NotificationTypeRepository : CrudRepository<NotificationType>, INotificationTypeRepository
{
    public NotificationTypeRepository(AppDbContext context) : base(context, "NotificationType") { }

    public async Task<List<NotificationType>> GetByRoleAsync(Role role)
    {
        var filter = Builders<NotificationType>.Filter.Eq(nt => nt.Role, role);

        var result = await _collection.Find(filter).ToListAsync();

        return result;
    }

    public async Task<List<NotificationType>> GetByIdsAsync(IEnumerable<int> ids)
    {
        var filter = Builders<NotificationType>.Filter.In(nt => nt.Id, ids);
        return await _collection.Find(filter).ToListAsync();
    }
    

}
