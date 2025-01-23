using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using TaskManagementMVC.DTOs;
using TaskManagementMVC.Services.Abstract;

namespace TaskManagementMVC.Controllers;
public class TaskController : Controller
{
    private readonly ITaskService _taskService;
    private readonly string ErrorMessage = "Hata oluştu!";
    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> All()
    {
        var result = await _taskService.GetAllTasksAsync();

        if (User.IsInRole("Admin"))
        {
            var usersResult = await _taskService.GetAllUsersAsync();

            if (!usersResult.IsSuccess)
            {
                return Redirect("/");
            }

            var users = usersResult.Data;

            var userSelectList = new SelectList(users, "Id", "Username");
            ViewBag.UserSelectList = userSelectList;

            return View(result.Data);
        }

        else if (User.IsInRole("User"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var filteredTasks = result.Data.Where(task => task.UserId == userId).ToList();

            return View(filteredTasks);
        }
       
        return Redirect("/Auth/Forbidden");
        
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var usersResult = await _taskService.GetAllUsersAsync();

        if (!usersResult.IsSuccess)
        {
            return Redirect("/");
        }

        var users = usersResult.Data;

        var userSelectList = new SelectList(users, "Id", "Username");
        ViewBag.UserSelectList = userSelectList;

        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add([FromForm] AddTaskDto model )
    {
        var usersResult = await _taskService.GetAllUsersAsync();

        if (!usersResult.IsSuccess)
        {
            return Redirect("/");
        }

        var users = usersResult.Data;

        var userSelectList = new SelectList(users, "Id", "Username");
        ViewBag.UserSelectList = userSelectList;

        if (!ModelState.IsValid)
        {
            ViewData["error"] = "Lütfen form verilerini istenen biçimde doldurunuz!..";
            return View(model);
        }

        var result = await _taskService.AddAsync(model);

        if (!result.IsSuccess)
        {
            ViewData["error"] = result.Message;
            return View(model);
        }

        ViewData["success"] = result.Message;
        return View();
    }

    [Authorize]
    [HttpGet("task-details/{taskId}")]
    public async Task<IActionResult> TaskDetails([FromRoute] string taskId)
    {
        var result = await _taskService.GetSingleTaskAsync(taskId);

        if (User.IsInRole("Admin") && result.Data.UserName is null)
        {
            var usersResult = await _taskService.GetAllUsersAsync();

            if (!usersResult.IsSuccess)
            {
                return Redirect("/");
            }

            var users = usersResult.Data;

            var userSelectList = new SelectList(users, "Id", "Username");
            ViewBag.UserSelectList = userSelectList;
        }

        return View(result.Data);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDto model)
    {
        var result = await _taskService.AddQuestionAsync(model);

        if (!result.IsSuccess)
        {
            return StatusCode(500, result.Message);
        }

        return Ok(new { message = result.Message });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ReplyQuestion([FromBody] ReplyQuestionDto model)
    {
        var result = await _taskService.ReplyQuestionAsync(model);

        if (!result.IsSuccess)
        {
            return StatusCode(500, result.Message);
        }

        return Ok(new { message = result.Message });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> MarkAsCompleted([FromForm] string taskId)
    {
        var refererUrl = HttpContext.Request.Headers["Referer"].ToString();

        var result = await _taskService.MarkAsCompletedAsync(taskId);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return Redirect(refererUrl);
        }

        TempData["success"] = result.Message;
        return Redirect(refererUrl);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> MarkAsOngoing([FromForm] string taskId)
    {
        var refererUrl = HttpContext.Request.Headers["Referer"].ToString();

        var result = await _taskService.MarkAsOngoingAsync(taskId);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return Redirect(refererUrl);
        }

        TempData["success"] = result.Message;
        return Redirect(refererUrl);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteTask([FromForm] string taskId)
    {
        var result = await _taskService.DeleteTaskAsync(taskId);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return RedirectToAction("All");
        }

        TempData["success"] = result.Message;
        return RedirectToAction("All");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AssignTask([FromForm] AssignTaskDto model)
    {
        var refererUrl = HttpContext.Request.Headers["Referer"].ToString();

        var result = await _taskService.AssignTaskAsync(model);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return Redirect(refererUrl);
        }

        TempData["success"] = result.Message;
        return Redirect(refererUrl);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> UpdateTask([FromQuery] string taskId)
    {
        var result = await _taskService.GetSingleTaskAsync(taskId);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return RedirectToAction("All");
        }

        var model = new UpdateTaskDto
        {
            Id = taskId,
            Description = result.Data.Description,
            Title = result.Data.Title,
            EndDate = result.Data.EndDate,
        };

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateTask([FromForm] UpdateTaskDto model)
    {
        var result = await _taskService.UpdateTaskAsync(model);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return RedirectToAction("All");
        }

        TempData["success"] = result.Message;
        return Redirect($"/task-details/{model.Id}");
    }
}
