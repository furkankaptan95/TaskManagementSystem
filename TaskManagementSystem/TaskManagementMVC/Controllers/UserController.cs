using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles ="Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Add(AddUserDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _userService.AddUserAsync(model);

        if (!result.IsSuccess)
        {
            ViewData["error"] = result.Message;
            return View(model);
        }

        ViewData["success"] = result.Message;
        return View();
    }

    [Authorize]
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> DeleteUser([FromQuery] string userId)
    {
        var result = await _userService.DeleteUserAsync(userId);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return RedirectToAction("All");
        }

        TempData["success"] = result.Message;
        return RedirectToAction("All");
    }

    [Authorize]
    [HttpGet("edit-user/{userId}")]
    public async Task<IActionResult> Update([FromRoute] string userId)
    {
        var result = await _userService.GetUserDetailsAsync(userId);

        if (!result.IsSuccess)
        {
            return Redirect("/");
        }

        var user = result.Data;

        return View(user);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
           .SelectMany(v => v.Errors)
           .Select(e => e.ErrorMessage)
           .ToList();

            TempData["error"] = string.Join("\n", errors);

            return Redirect($"/user-details/{updateUserDto.Id}");
        }

        var result = await _userService.UpdateUserAsync(updateUserDto);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return Redirect($"/user-details/{updateUserDto.Id}");
        }

        TempData["success"] = result.Message;
        return Redirect($"/user-details/{updateUserDto.Id}");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ChangeRole([FromForm] UpdateRoleDto updateRoleDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            TempData["error"] = string.Join("\n", errors);
            return Redirect($"/user-details/{updateRoleDto.UserId}");
        }

        var result = await _userService.UpdateRoleAsync(updateRoleDto);

        if (!result.IsSuccess)
        {
            TempData["error"] = result.Message;
            return Redirect($"/user-details/{updateRoleDto.UserId}");
        }

        TempData["success"] = result.Message;
        return Redirect($"/user-details/{updateRoleDto.UserId}");
    }
}
