using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UserAPI.Entities;
public class UserEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Role { get; set; } = "User";
}
