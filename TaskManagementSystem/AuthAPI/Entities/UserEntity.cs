using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AuthAPI.Entities;
public class UserEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public string Role { get; set; } = "worker";

    [BsonIgnore]
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
}
