using Microsoft.AspNetCore.Mvc;
using TaskManagementMVC.DTOs;
using TaskManagementMVC.Services.Abstract;

namespace TaskManagementMVC.Controllers;
public class UserController : Controller
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> All()
    {
        var result = await _userService.GetAllUsersAsync();

        if (!result.IsSuccess)
        {
            return Redirect("/");
        }

        var users = result.Data;

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddUserDto model)
    {
        var result = await _userService.AddUserAsync(model);

        if (!result.IsSuccess)
        {
            ViewData["error"] = result.Message;
            return View(model);
        }

        ViewData["success"] = result.Message;
        return View();
    }

    [HttpGet("user-details/{userId}")]
    public async Task<IActionResult> UserDetails([FromRoute] string userId)
    {
        var result = await _userService.GetUserDetailsAsync(userId);

        if (!result.IsSuccess)
        {
            return Redirect("/");
        }

        var user = result.Data;

        return View(user);
    }
}
