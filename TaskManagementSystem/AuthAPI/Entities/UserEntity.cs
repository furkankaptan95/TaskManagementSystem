using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AuthAPI.Entities;
public class UserEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = false;
    public string Role { get; set; } = "worker";

    [BsonIgnore]
    public ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
}
