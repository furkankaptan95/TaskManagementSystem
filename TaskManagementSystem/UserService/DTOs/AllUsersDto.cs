namespace UserAPI.DTOs;
public class AllUsersDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; }
}
