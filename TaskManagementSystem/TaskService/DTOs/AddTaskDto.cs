namespace TaskAPI.DTOs;

public class AddTaskDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public string? UserId { get; set; }
    public DateTime EndDate { get; set; }
}
