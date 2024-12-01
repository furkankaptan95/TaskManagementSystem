using Microsoft.AspNetCore.Mvc;
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

    [HttpGet]
    public async Task<IActionResult> All()
    {
        var result = await _taskService.GetAllAsync();

        if (!result.IsSuccess)
        {
            ViewData["error"] = ErrorMessage;
            return View();
        }

        return View(result.Data);
    }

}
