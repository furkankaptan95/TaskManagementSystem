using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TaskAPI.Entities;
public class UserEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }

    [BsonIgnore]
    public  ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();
}
