
using NotificationService.Model.Entity;

namespace NotificationService.Model.Dto;


public class NotificationDto
{
    public string Id { get; set; }
    public Guid NotificationUserExternalId { get; set; }
    public string Message { get; set; }
    public bool Seen { get; set; } = false;
    public int NotificationTypeId { get; set; }
}