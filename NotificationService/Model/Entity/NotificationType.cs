
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.Model.Entity;


public class NotificationType
{
    [BsonId]
    public int Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonRepresentation(BsonType.String)]
    [BsonElement("role")]
    public Role Role { get; set; }
}