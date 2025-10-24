namespace NotificationService.Repository.Contract;

public interface IRepositoryManager
{
    public IUserRepository UserRepository { get; }
    public INotificationRepository NotificationRepository { get; }
    public INotificationTypeRepository NotificationTypeRepository { get; }
    public INotificationUserRepository NotificationUserRepository { get; }
}
