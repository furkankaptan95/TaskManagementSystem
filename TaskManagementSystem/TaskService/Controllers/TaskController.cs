﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TaskAPI.DTOs;
using TaskAPI.Services;

namespace TaskAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var taskDtos = await _taskService.GetAllTasksAsync();

        return Ok(taskDtos);
    }

    [Authorize]
    [HttpGet("{taskId}")]
    public async Task<IActionResult> GetById([FromRoute] string taskId)
    {
        if (!ObjectId.TryParse(taskId, out _))
        {
            return BadRequest("Invalid Task ID format.");
        }

        var result = await _taskService.GetTaskByIdAsync(taskId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Data);
    }

    [Authorize(Roles ="Admin")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddTaskDto addDto)
    {
        var result = await _taskService.AddTaskAsync(addDto);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] UpdateTaskDto updateTaskDto)
    {
        var result = await _taskService.UpdateTaskAsync(updateTaskDto);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete/{taskId}")]
    public async Task<IActionResult> Delete([FromRoute] string taskId)
    {
        if (!ObjectId.TryParse(taskId, out _))
        {
            return BadRequest("Invalid ID format.");
        }

        await _taskService.DeleteTaskAsync(taskId);

        return Ok("Task deleted successfully.");
    }

    [Authorize]
    [HttpGet("user-tasks/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] string userId)
    {
        if (!ObjectId.TryParse(userId, out _))
        {
            return BadRequest("Invalid UserId format.");
        }
        
        var userTasks = await _taskService.GetTasksByUserIdAsync(userId);

        return Ok(userTasks);
    }

    [Authorize]
    [HttpPut("status")]
    public async Task<IActionResult> ChangeStatus([FromBody] string taskId)
    {
        if (!ObjectId.TryParse(taskId, out _))
        {
            return BadRequest("Invalid ID format.");
        }

        var result = await _taskService.ChangeStatusAsync(taskId);

        if (!result.IsSuccess)
        {
            return NotFound(result.Message);
        }

        return Ok(result.Message);
    }

    [Authorize(Roles = "User")]
    [HttpPost("add-question")]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDto addQuestionDto)
    {
        var result = await _taskService.AddQuestionAsync(addQuestionDto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("reply-question")]
    public async Task<IActionResult> ReplyQuestion([FromBody] ReplyQuestionDto replyQuestionDto)
    {
        var result = await _taskService.ReplyQuestionAsync(replyQuestionDto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(result.Message);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("assign")]
    public async Task<IActionResult> AssignTaskToUser([FromBody] AssignTaskDto assignDto)
    {
        var result = await _taskService.AssignTaskToUserAsync(assignDto);

        return Ok(result.Message);
    }

    // Task Notifications (Görev Bildirimleri) endpointleri eklenecek.
}
