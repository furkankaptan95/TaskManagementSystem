﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using UserAPI.DTOs;
using UserAPI.Helpers;
using UserAPI.Services;

namespace UserAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly RabbitMQProducer _rabbitMQProducer;

    public UserController(IUserService userService, RabbitMQProducer rabbitMQProducer)
    {
        _userService = userService;
        _rabbitMQProducer = rabbitMQProducer;
    }

    [Authorize(Roles ="Admin")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var userDtos = await _userService.GetAllUsersAsync();

        return Ok(userDtos);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetById([FromRoute] string userId)
    {
        if (!ObjectId.TryParse(userId, out _))
        {
            return BadRequest("Invalid UserId format.");
        }

        var result = await _userService.GetUserByIdAsync(userId);

        if(!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Data);
    }

    [Authorize(Roles = "Admin")]
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

        _rabbitMQProducer.SendMessage(dto, "user_update_queue");

        return Ok(result.Message);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update-role")]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleDto dto)
    {
        if (!ObjectId.TryParse(dto.UserId, out _))
        {
            return BadRequest("Invalid UserId format.");
        }

        var result = await _userService.UpdateUserRoleAsync(dto);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        _rabbitMQProducer.SendMessage(dto, "user_role_update_queue"); 

        return Ok(result.Message);
    }

    [Authorize(Roles = "Admin")]
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

        _rabbitMQProducer.SendMessage(userId, "user_delete_queue_taskapi");
        _rabbitMQProducer.SendMessage(userId, "user_delete_queue_authapi");

        return Ok(result.Message);
    }
}
