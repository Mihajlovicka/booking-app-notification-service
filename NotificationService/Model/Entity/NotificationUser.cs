
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Model.Entity;


public class NotificationUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("notificationTypeId")]
    public int NotificationTypeId { get; set; }

    [BsonElement("userId")]
    public string UserExternalId { get; set; }
}