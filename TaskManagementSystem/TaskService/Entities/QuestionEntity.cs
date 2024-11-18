using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskAPI.Entities;
public class QuestionEntity
{
    [BsonId]
    public ObjectId Id { get; set; }
    public string Content { get; set; }
    public string? Answer { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AnsweredAt { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string TaskId { get; set; }
    [BsonIgnore]
    public TaskEntity Task { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }
    [BsonIgnore]
    public UserEntity User { get; set; }
}
