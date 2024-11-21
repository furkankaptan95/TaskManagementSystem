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
    public async Task<IActionResult> RefreshTokenAsync([FromRoute] string token)
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

}
