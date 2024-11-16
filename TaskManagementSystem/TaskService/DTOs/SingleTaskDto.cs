namespace TaskAPI.DTOs;
public class SingleTaskDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime EndDate { get; set; }
    public string? UserId { get; set; }
    public string? TaskUserName { get; set; }
}
