using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface IUserService
{
    Task<ServiceResult<List<AllUsersDto>>> GetAllUsersAsync();
    Task<ServiceResult> AddUserAsync(AddUserDto dto);
}
