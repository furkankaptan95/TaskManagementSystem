using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TaskAPI.DTOs;
using TaskAPI.Services;

namespace TaskAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskService _taskService;

    public TaskController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet("tasks")]
    public async Task<IActionResult> GetAll()
    {
        var taskDtos = await _taskService.GetAllTasksAsync();

        return Ok(taskDtos);
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetDetails([FromRoute] string taskId)
    {
        if (!ObjectId.TryParse(taskId, out _))
        {
            return BadRequest("Invalid ID format.");
        }

        var dto = await _taskService.GetTaskByIdAsync(taskId);

        return Ok(dto);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddTaskDto addDto)
    {
        await _taskService.AddTaskAsync(addDto);

        return Ok();
    }

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

    [HttpDelete("delete/{taskId}")]
    public async Task<IActionResult> Delete([FromRoute] string taskId)
    {
        if (!ObjectId.TryParse(taskId, out _))
        {
            return BadRequest("Invalid ID format.");
        }

        await _taskService.DeleteTaskAsync(taskId);

        return Ok();
    }

    [HttpGet("user-tasks/{userId}")]
    public async Task<IActionResult> GetByUserId([FromRoute] string userId)
    {
        return Ok();
    }

    [HttpPut("status/{taskId}")]
    public async Task<IActionResult> ChangeStatus([FromRoute] string taskId)
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

    [HttpPost("reply-question")]
    public async Task<IActionResult> ReplyQuestion([FromBody] object ReplyQuestionDto)
    {
        return Ok();
    }
}
