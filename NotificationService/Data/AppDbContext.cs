using MongoDB.Driver;

namespace NotificationService.Data;

public class AppDbContext
{
    public readonly IMongoDatabase Database;

    public AppDbContext(IMongoDatabase database)
    {
        Database = database;
    }

    // public IMongoCollection<Notification> Notifications =>
    //     _database.GetCollection<Notification>("Notifications");

    // // You can add more collections here as needed
}
