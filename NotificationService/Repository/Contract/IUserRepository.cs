using NotificationService.Model.Entity;

namespace NotificationService.Repository.Contract;

public interface IUserRepository : ICrudRepository<User>
{
    Task<User?> GetByExternalIdAsync(Guid externalId);
    Task<User?> GetByUsernameAsync(string username);
    Task DeleteAllForUserAsync(string id);
}

