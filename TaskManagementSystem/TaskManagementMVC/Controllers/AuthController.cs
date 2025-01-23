using Microsoft.AspNetCore.Mvc;

namespace TaskManagementMVC.Controllers;
public class AuthController : Controller
{
    public IActionResult Forbidden()
    {
        return View();
    }
}
