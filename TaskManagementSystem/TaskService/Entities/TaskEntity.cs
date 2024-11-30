using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskAPI.Entities;
public class TaskEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? AssignedAt { get; set; }
    public string? UserId { get; set; }

    [BsonIgnore]
    public UserEntity? User { get; set; }

    [BsonIgnore]
    public ICollection<QuestionEntity> Questions { get; set; } = new List<QuestionEntity>();

}
