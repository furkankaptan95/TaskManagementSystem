using Microsoft.AspNetCore.Mvc;

namespace TaskManagementMVC.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
