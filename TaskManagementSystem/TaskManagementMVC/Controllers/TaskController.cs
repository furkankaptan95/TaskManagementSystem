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
            return View("AdminAll", result.Data);
        }

        else if (User.IsInRole("User"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var filteredTasks = result.Data.Where(task => task.UserId == userId).ToList();

            return View("UserAll", filteredTasks);
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

    [HttpGet("{taskId}")]
    public async Task<IActionResult> TaskDetails([FromRoute] string taskId)
    {
        var result = await _taskService.GetSingleTaskAsync(taskId);

        return View(result.Data);
    }
}
