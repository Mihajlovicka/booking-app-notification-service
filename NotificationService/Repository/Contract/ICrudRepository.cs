namespace NotificationService.Repository.Contract;

public interface ICrudRepository<T>
    where T : class
{
    // Retrieve all entities
    Task<IEnumerable<T>> GetAllAsync();

    // Retrieve a single entity by its ID
    Task<T> GetByIdAsync(string id);

    // Add a new entity
    Task AddAsync(T entity);

    // Update an existing entity
    Task UpdateAsync(string id, T entity);

    // Delete an entity by its ID
    Task DeleteAsync(string id);
}
