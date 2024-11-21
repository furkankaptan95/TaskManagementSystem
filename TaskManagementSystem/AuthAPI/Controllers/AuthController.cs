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
    public async Task<IActionResult> ValidateTokenAsync([FromBody] string token)
    {
        var result = _authService.ValidateToken(token);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }
}
