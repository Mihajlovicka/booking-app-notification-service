using NotificationService.Model.Dto;
using NotificationService.Model.Messages;

namespace NotificationService.Service.MessagingService;

public static class TopicTypeMap
{
    public static readonly Dictionary<KafkaTopic, Type> Map =
        new() { 
            { KafkaTopic.UserCreated, typeof(UserDto) },
            { KafkaTopic.DeleteUser, typeof(UserDto) },
            { KafkaTopic.NotificationCreated, typeof(NotificationDto) } 
        };
}

public enum KafkaTopic
{
    UserCreated,
    DeleteUser,
    NotificationCreated
}
