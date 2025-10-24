
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Model.Dto;

namespace NotificationService.Model.Entity;


public class Notification
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("notificationUserId")]
    public string NotificationUserExternalId { get; set; }

    [BsonElement("notificationTypeId")]
    public int NotificationTypeId { get; set; }

    [BsonElement("message")]
    public string Message { get; set; }

    [BsonElement("seen")]
    public bool Seen { get; set; }

    public Notification() { }

    public Notification(NotificationDto dto)
    {
        NotificationTypeId = dto.NotificationTypeId;
        Message = dto.Message;
        Seen = false;
    }
}