using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthAPI.Entities;
public class UserVerificationEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; } = DateTime.UtcNow.AddHours(24);
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonIgnore]
    public UserEntity User { get; set; }
}
