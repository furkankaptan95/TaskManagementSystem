using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using TaskAPI.Entities;

namespace UserAPI.Entities;
public class UserEntity
{
    // MongoDB'nin otomatik olarak ürettiği _id alanını işaret ederiz
    [BsonId]
    public ObjectId Id { get; set; }  // MongoDB'deki ObjectId türünde olacak
    public string Username { get; set; }
    public string Email { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string Role { get; set; } = "worker";
    [BsonIgnore]
    public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
