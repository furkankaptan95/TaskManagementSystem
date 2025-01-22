using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface IUserService
{
    Task<ServiceResult<List<AllUsersDto>>> GetAllUsersAsync();
    Task<ServiceResult> AddUserAsync(AddUserDto dto);
    Task<ServiceResult> UpdateUserAsync(UpdateUserDto dto);
    Task<ServiceResult<UserDetailsDto>> GetUserDetailsAsync(string userId);
    Task<ServiceResult> DeleteUserAsync(string userId);
}
