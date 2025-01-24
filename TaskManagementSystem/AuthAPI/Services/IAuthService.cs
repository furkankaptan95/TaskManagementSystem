using AuthAPI.DTOs;

namespace AuthAPI.Services;
public interface IAuthService
{
    Task<RegistrationResult> RegisterAsync(RegisterDto dto);
    Task<ServiceResult<TokensDto>> LoginAsync(LoginDto dto);
    Task<ServiceResult<TokensDto>> RefreshTokenAsync(string token);
    Task<ServiceResult> VerifyEmailAsync(VerifyEmailDto dto);
    ServiceResult ValidateToken(string token);
    Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<ServiceResult<string>> RenewPasswordVerifyEmailAsync(RenewPasswordDto dto);
    Task<ServiceResult> NewPasswordAsync(NewPasswordDto dto);
    Task<ServiceResult> NewVerificationAsync(NewVerificationMailDto dto);
    Task<ServiceResult> RevokeTokenAsync(string token);
}
