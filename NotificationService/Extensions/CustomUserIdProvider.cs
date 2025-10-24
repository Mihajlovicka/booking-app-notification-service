
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Extensions;
public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        // use username as identifier instead of default NameIdentifier
        var v = connection.User?.FindFirst("name")?.Value;
        return v;
    }
}