using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskManagementMVC.Models;

namespace TaskManagementMVC.Controllers;
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
