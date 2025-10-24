using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Model.Entity;

namespace NotificationService.Extensions;

public class NotificationSeeder
{
    private readonly IMongoCollection<NotificationType> _collection;

    public NotificationSeeder(AppDbContext context)
    {
        _collection = context.Database.GetCollection<NotificationType>("NotificationType");
    }

    public async Task SeedAsync()
    {
        if (await _collection.CountDocumentsAsync(FilterDefinition<NotificationType>.Empty) > 0)
            return;

        var types = new List<NotificationType>
        {
            new() { Id = 1, Name = "Create Reservation", Role = Role.HOST },
            new() { Id = 2, Name = "Cancel Reservation", Role = Role.HOST },
            new() { Id = 3, Name = "New rate on host", Role = Role.HOST },
            new() { Id = 4, Name = "New rate on accommodation", Role = Role.HOST },
            new() { Id = 5, Name = "ReservationProcessed", Role = Role.GUEST }
        };

        await _collection.InsertManyAsync(types);
    }
}