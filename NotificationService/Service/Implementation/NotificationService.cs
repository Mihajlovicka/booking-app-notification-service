
using NotificationService.Data.Dto;
using NotificationService.Model.Dto;
using NotificationService.Model.Entity;
using NotificationService.Repository.Contract;
using NotificationService.Service.Contract;

namespace NotificationService.Service.Implementation;

public class NotificationService(IUserContext userContext, IRepositoryManager repositoryManager) : INotificationService
{
    public async Task<IEnumerable<NotificationTypeDto>> GetAllByRole()
    {
        var role = (Role)Enum.Parse(typeof(Role), userContext.Role);
        var typesByRole = await repositoryManager.NotificationTypeRepository.GetByRoleAsync(role);
        var dtos = typesByRole
                .Select(nt => new NotificationTypeDto(nt))
                .ToList();
        return dtos;
    }

    public async Task<IEnumerable<NotificationTypeDto>> GetAllByCurrentUser()
    {
        var role = (Role)Enum.Parse(typeof(Role), userContext.Role);
        var user = await repositoryManager.UserRepository.GetByUsernameAsync(userContext.Name);

        var notificationUsers = await repositoryManager.NotificationUserRepository.GetByUserIdAsync(user.Id);
        var notificationTypeIds = notificationUsers.Select(nu => nu.NotificationTypeId).ToList();
        var notificationTypes = await repositoryManager.NotificationTypeRepository.GetByIdsAsync(notificationTypeIds);
        var dtos = notificationTypes
            .Select(nt => new NotificationTypeDto(nt))
            .ToList();

        return dtos;
    }

    public async Task<IEnumerable<NotificationTypeDto>> Save(NotificationTypeDto[] notifications)
    {
        // 1. Get the user
        var user = await repositoryManager.UserRepository.GetByUsernameAsync(userContext.Name);
        if (user == null)
            throw new Exception($"User '{userContext.Name}' not found.");

        // 2. Delete all existing NotificationUser for this user
        await repositoryManager.NotificationUserRepository.DeleteByUserIdAsync(user.Id);

        // 3. Get NotificationType objects for the provided DTOs
        var notificationTypeIds = notifications
            .Where(n => n.Id != null)
            .Select(n => n.Id)
            .ToList();

        var notificationTypes = await repositoryManager.NotificationTypeRepository.GetByIdsAsync(notificationTypeIds);

        // 4. Create new NotificationUser list
        var notificationUsers = notificationTypes.Select(nt => new NotificationUser
        {
            NotificationTypeId = nt.Id,
            UserExternalId = user.Id
        }).ToList();

        // 5. Save all NotificationUsers
        if (notificationUsers.Any())
            await repositoryManager.NotificationUserRepository.AddManyAsync(notificationUsers);

        // 6. Return the DTOs that were saved
        return notificationTypes.Select(nt => new NotificationTypeDto(nt)).ToList();
    }

    public async Task<IEnumerable<NotificationDto>> GetOldNotifications()
    {
        var user = await repositoryManager.UserRepository.GetByUsernameAsync(userContext.Name);
        if (user == null)
            throw new Exception($"User '{userContext.Name}' not found.");

        var notifications = await repositoryManager.NotificationRepository.GetByNotificationUserIdAsync(user.ExternalId.ToString());

        return notifications
            .Select(n => new NotificationDto
            {
                Message = n.Message,
                Seen = n.Seen,
                NotificationUserExternalId = Guid.Parse(n.NotificationUserExternalId),
                NotificationTypeId = n.NotificationTypeId
            })
            .ToList();
    }

    public async Task<IEnumerable<NotificationDto>> UpdateOldNotifications(IEnumerable<NotificationDto> notificationDtos)
    {
        var user = await repositoryManager.UserRepository.GetByUsernameAsync(userContext.Name);
        if (user == null)
            throw new Exception($"User '{userContext.Name}' not found.");

        // Delete all existing notifications for this user
        await repositoryManager.NotificationRepository.DeleteByUserExternalIdAsync(user.ExternalId.ToString());

        // Map DTOs to entities
        var notifications = notificationDtos.Select(dto => new Notification
        {
            Message = dto.Message,
            Seen = dto.Seen,
            NotificationUserExternalId = dto.NotificationUserExternalId.ToString(),
            NotificationTypeId = dto.NotificationTypeId
        }).ToList();

        // Save all new notifications
        if (notifications.Any())
            await repositoryManager.NotificationRepository.AddManyAsync(notifications);

        // Return saved notifications as DTOs
        return notifications.Select(n => new NotificationDto
        {
            Message = n.Message,
            Seen = n.Seen,
            NotificationUserExternalId = Guid.Parse(n.NotificationUserExternalId),
            NotificationTypeId = n.NotificationTypeId
        }).ToList(); 
    }
}