using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Model.Entity;
using NotificationService.Service.MessagingService;

namespace NotificationService.Extensions;
public class MongoMigrationRunner
{
    private readonly AppDbContext _context;
    private readonly NotificationSeeder _seeder;

    public MongoMigrationRunner(AppDbContext context, NotificationSeeder seeder)
    {
        _context = context;
        _seeder = seeder;
    }

    public async Task RunAsync()
    {
        var db = _context.Database;

        // Ensure required collections
        var collections = await db.ListCollectionNames().ToListAsync();
        var neededCollections = new[] { "User", "NotificationType", "NotificationUser", "Notification" };

        foreach (var name in neededCollections)
        {
            if (!collections.Contains(name))
            {
                Console.WriteLine($"[MongoMigrationRunner] Creating collection {name}...");
                await db.CreateCollectionAsync(name);
            }
            else
            {
                Console.WriteLine($"[MongoMigrationRunner] Collection {name} already exists.");
            }
        }

        // Ensure unique index on User.ExternalId
        var userCollection = db.GetCollection<User>("User");
        var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.ExternalId);
        var indexModel = new CreateIndexModel<User>(indexKeys, new CreateIndexOptions { Unique = true });
        await userCollection.Indexes.CreateOneAsync(indexModel);

        Console.WriteLine("[MongoMigrationRunner] Index created on User.ExternalId");

        await _seeder.SeedAsync();
        Console.WriteLine("[MongoMigrationRunner] Seeding done.");
    }
}