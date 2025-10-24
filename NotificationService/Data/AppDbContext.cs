using MongoDB.Driver;

namespace NotificationService.Data;
public class AppDbContext
    {
        public IMongoDatabase Database { get; }

        public AppDbContext(IConfiguration configuration)
    {
            
            // Try to get connection string from env or appsettings
            var connectionString = configuration.GetConnectionString("MongoDb") 
                                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__MongoDb")
                                   ?? "mongodb://localhost:27017";

            // Try to get DB name from env or appsettings
            var databaseName = configuration["MongoDatabaseName"] 
                               ?? Environment.GetEnvironmentVariable("MongoDatabaseName") 
                               ?? "NotificationDb";

            Console.WriteLine($"[AppDbContext] Connecting to MongoDB: {connectionString}, DB: {databaseName}");

            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(databaseName);
        }
    }