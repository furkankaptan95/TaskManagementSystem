﻿using TaskManagementMVC.DTOs;

namespace TaskManagementMVC.Services.Abstract;
public interface IAuthService
{
    Task<ServiceResult<TokensDto>> LoginAsync(LoginDto loginDto);
    Task<ServiceResult<TokensDto>> RefreshTokenAsync(string token);
    Task<ServiceResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<ServiceResult<string>> RenewPasswordEmailAsync(RenewPasswordDto dto);
    Task<ServiceResult> NewPasswordAsync(NewPasswordDto dto);
    Task<ServiceResult> RevokeTokenAsync(string token);
}
