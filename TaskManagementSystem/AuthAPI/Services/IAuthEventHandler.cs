using AuthAPI.DTOs;

namespace AuthAPI.Services;
public interface IAuthEventHandler
{
    Task<ServiceResult> HandleUserUpdateAsync(UpdateUserDto updateUserDto);
    Task<ServiceResult> HandleUserRoleUpdateAsync(UpdateRoleDto updateRoleDto);
    Task<ServiceResult> HandleDeleteUserAsync(string userId);
}
