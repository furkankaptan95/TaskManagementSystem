using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AuthAPI.Entities;
public class RefreshTokenEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpireDate { get; set; }
    public bool IsUsed { get; set; } = false;
    public bool IsRevoked { get; set; } = false;
    public string UserId { get; set; }

    [BsonIgnore]
    public UserEntity User { get; set; }
}
