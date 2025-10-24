using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Repository.Contract;

namespace NotificationService.Repository.Implementation;

public class CrudRepository<T> : ICrudRepository<T> where T : class
{
    protected readonly IMongoCollection<T> _collection;

    public CrudRepository(AppDbContext context, string collectionName = null)
    {
        _collection = context.Database.GetCollection<T>(collectionName ?? typeof(T).Name);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
        => await _collection.Find(FilterDefinition<T>.Empty).ToListAsync();

    public virtual async Task<T> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public virtual async Task AddAsync(T entity)
        => await _collection.InsertOneAsync(entity);

    public virtual async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public virtual async Task DeleteAllForUserAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await _collection.DeleteOneAsync(filter);
    }
}
