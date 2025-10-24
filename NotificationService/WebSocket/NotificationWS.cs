
using Microsoft.AspNetCore.SignalR;
using NotificationService.Model.Dto;

namespace NotificationService.WebSocket;
public class NotificationService(IHubContext<NotificationHub> _hubContext, ILogger<NotificationService> _logger)
{
    public async Task SendNotificationAsync(string userId, NotificationDto notificationDto)
    {
        await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", new
        {
            Id = notificationDto.Id,
            Message = notificationDto.Message,
            Seen = false
        });

        _logger.LogInformation($"Notification sent to user {userId} via WebSocket.");
    }
}
