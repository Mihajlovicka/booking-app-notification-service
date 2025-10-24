using NotificationService.Repository.Contract;

namespace NotificationService.Repository.Implementation;

public class RepositoryManager(
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    INotificationTypeRepository notificationTypeRepository,
    INotificationUserRepository notificationUserRepository) : IRepositoryManager
{
    public IUserRepository UserRepository { get; } = userRepository;
    public INotificationRepository NotificationRepository { get; } = notificationRepository;
    public INotificationTypeRepository NotificationTypeRepository { get; } = notificationTypeRepository;
    public INotificationUserRepository NotificationUserRepository { get; } = notificationUserRepository;
}
