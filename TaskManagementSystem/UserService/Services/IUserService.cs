using UserAPI.DTOs;

namespace UserAPI.Services;
public interface IUserService
{
    Task<List<AllUsersDto>> GetAllUsersAsync();
    Task<ServiceResult<SingleUserDto>> GetUserByIdAsync(string id);
    Task<ServiceResult> UpdateUserAsync(UpdateUserDto dto);
    Task<ServiceResult> DeleteUserAsync(string id);
    Task<ServiceResult> UpdateUserRoleAsync(UpdateRoleDto dto);
}
