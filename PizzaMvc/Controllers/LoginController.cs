using Microsoft.AspNetCore.Mvc;

namespace PizzaMvc.Controllers;

public class LoginController : Controller
{
    public IActionResult Index()
    {
        return View("login");
    }
}
