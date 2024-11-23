namespace TaskAPI.DTOs;
public class QuestionDto
{
    public string Id { get; set; }
    public string Content { get; set; }
    public string? Answer { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AnsweredAt { get; set; }
    public string TaskId { get; set; }
    public string TaskTitle { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
}
