using Microsoft.AspNetCore.Mvc;

namespace PizzaMvc.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
