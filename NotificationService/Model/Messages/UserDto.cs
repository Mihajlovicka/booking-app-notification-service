namespace NotificationService.Model.Messages;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? Role { get; set; }
}