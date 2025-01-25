using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface IAuthService
{
    Task<ServiceResult<TokensDto>> LoginAsync(LoginDto loginDto);
}
