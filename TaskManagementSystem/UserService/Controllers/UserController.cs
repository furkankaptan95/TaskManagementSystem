using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
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

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserDto dto)
    {
        if (!ObjectId.TryParse(dto.Id, out _))
        {
            return BadRequest("Invalid UserId format.");
        }

        var result = await _userService.UpdateUserAsync(dto);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpDelete("delete/{userId}")]
    public async Task<IActionResult> Delete([FromRoute] string userId)
    {
        if (!ObjectId.TryParse(userId, out _))
        {
            return BadRequest("Invalid UserId format.");
        }

        var result = await _userService.DeleteUserAsync(userId);
        
        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] NewPasswordDto dto)
    {
        var result = await _userService.ChangePasswordAsync(dto);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }
}
