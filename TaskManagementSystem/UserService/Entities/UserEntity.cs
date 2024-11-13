﻿namespace UserService.Entities;
public class UserEntity
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public string Role { get; set; }
    public virtual ICollection<Task> Tasks { get; set; }
}