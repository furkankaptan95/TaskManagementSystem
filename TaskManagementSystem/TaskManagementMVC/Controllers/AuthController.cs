﻿using Microsoft.AspNetCore.Mvc;
using TaskManagementMVC.DTOs;
using TaskManagementMVC.Services.Abstract;

namespace TaskManagementMVC.Controllers;
public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    public IActionResult Forbidden()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        var result = await _authService.LoginAsync(loginDto);

        if (!result.IsSuccess)
        {
            ViewData["error"] = result.Message;
            return View(loginDto);
        }

        CookieOptions jwtCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(10)
        };

        CookieOptions refreshTokenCookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        HttpContext.Response.Cookies.Append("JwtToken", result.Data.JwtToken, jwtCookieOptions);
        HttpContext.Response.Cookies.Append("RefreshToken", result.Data.RefreshToken, refreshTokenCookieOptions);

        TempData["success"] = result.Message;

        return Redirect("/");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var result = await _authService.ForgotPasswordAsync(dto);
        if (!result.IsSuccess)
        {
            ViewData["error"] = result.Message;
            return View(dto);
        }

        ViewData["success"] = result.Message;
        return View();
    }


    [HttpGet("renew-password")]
    public async Task<IActionResult> RenewPassword([FromQuery] string email, string token)
    {
       
            var dto = new RenewPasswordDto(email, token);

            var result = await _authService.RenewPasswordEmailAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["error"] = result.Message;
                return RedirectToAction(nameof(ForgotPassword));
            }

            ViewData["success"] = result.Message;

            var model = new NewPasswordDto
            {
                Token = result.Data,
                Email = email
            };

            return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RenewPasswordPost([FromForm] NewPasswordDto dto )
    {
        var result = await _authService.NewPasswordAsync(dto);

        if (result.IsSuccess)
        {
            TempData["success"] = result.Message;
            return RedirectToAction(nameof(Login));
        }

        TempData["error"] = result.Message;
        return RedirectToAction(nameof(ForgotPassword));
    }


    [HttpGet]
    public async Task<IActionResult> LogOut()
    {
        var refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            Response.Cookies.Delete("JwtToken");

            TempData["success"] = "Hesabınızdan başarıyla çıkış yapıldı.";
            return Redirect("/Auth/Login");

        }

        var result = await _authService.RevokeTokenAsync(refreshToken);

        if (result.IsSuccess)
        {
            Response.Cookies.Delete("JwtToken");
            Response.Cookies.Delete("RefreshToken");

            TempData["success"] = result.Message;
            return Redirect("/Auth/Login");
        }

        TempData["error"] = result.Message;
        return Redirect("/");
    }

}
