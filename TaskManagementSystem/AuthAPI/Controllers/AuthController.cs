using AuthAPI.DTOs;
using AuthAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if(!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result);
    }

    [HttpGet("refresh-token/{token}")]
    public async Task<IActionResult> RefreshToken([FromRoute] string token)
    {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token null or empty.");
            }

            var result = await _authService.RefreshTokenAsync(token);

            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto)
    {
        var result = await _authService.VerifyEmailAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message); 
        }

        return Ok(result.Message);
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        var result = _authService.ValidateToken(token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {      
        var result = await _authService.ForgotPasswordAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost("renew-password-verify")]
    public async Task<IActionResult> RenewPassword([FromBody] RenewPasswordDto dto)
    {
        var result = await _authService.RenewPasswordVerifyEmailAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost("new-password")]
    public async Task<IActionResult> NewPasswordAsync([FromBody] NewPasswordDto dto)
    {
        var result = await _authService.NewPasswordAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPost("new-verification")]
    public async Task<IActionResult> NewVerificationAsync([FromBody] NewVerificationMailDto dto)
    {
        var result = await _authService.NewVerificationAsync(dto);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }
}
