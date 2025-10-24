using Microsoft.AspNetCore.SignalR;
using NotificationService.Model.Entity;

namespace NotificationService.WebSocket;

public class NotificationHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"User connected: {Context.UserIdentifier}");
        return base.OnConnectedAsync();
    }
}
