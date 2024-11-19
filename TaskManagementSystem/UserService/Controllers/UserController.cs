using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Threading.Tasks;
using UserAPI.DTOs;
using UserAPI.Services;

namespace UserAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var userDtos = await _userService.GetAllUsersAsync();

        return Ok(userDtos);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        await _userService.CreateUserAsync(dto);

        return Ok("User created successfully.");
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetById([FromRoute] string userId)
    {
        if (!ObjectId.TryParse(userId, out _))
        {
            return BadRequest("Invalid UserId format.");
        }

        var user = await _userService.GetUserByIdAsync(userId);

        if(user is null)
        {
            return NotFound("User not found.");
        }

        return Ok(user);
    }
}
