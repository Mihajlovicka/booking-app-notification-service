using NotificationService.Model.Entity;

namespace NotificationService.Data.Dto;

public class NotificationTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public NotificationTypeDto() { }

    public NotificationTypeDto(NotificationType sr)
    {
        Name = sr.Name;
        Id = sr.Id;
    }
}