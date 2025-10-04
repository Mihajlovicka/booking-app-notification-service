using MongoDB.Driver;
using NotificationService.Data;

namespace NotificationService.Extensions;

public static class MongoDbExtensions
{
    public static void EnsureDatabaseSetup(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // var notifications = dbContext.Notifications;
        // var indexKeys = Builders<Notification>.IndexKeys.Ascending(n => n.CreatedAt);
        // var indexModel = new CreateIndexModel<Notification>(indexKeys);
        // notifications.Indexes.CreateOne(indexModel);
    }
}
