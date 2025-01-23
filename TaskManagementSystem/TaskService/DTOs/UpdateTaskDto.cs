namespace TaskAPI.DTOs;
public class UpdateTaskDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime EndDate { get; set; }
}
