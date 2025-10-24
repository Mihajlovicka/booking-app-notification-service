using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using NotificationService.Model.Messages;

namespace NotificationService.Model.Entity;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("username")]
    public string Username { get; set; }

    [BsonElement("externalId")]
    [BsonRepresentation(BsonType.String)]
    public Guid ExternalId { get; set; }

    [BsonRepresentation(BsonType.String)]
    [BsonElement("role")]
    public Role Role { get; set; }

    public User() { }

    public User(UserDto dto)
    {
        Username = dto.Username;
        ExternalId = dto.Id;
        Role = (Role)Enum.Parse(typeof(Role), dto.Role);
    }
}