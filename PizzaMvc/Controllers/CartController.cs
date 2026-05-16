using Microsoft.AspNetCore.Mvc;

namespace PizzaMvc.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        public IActionResult Index()
        {
            return View();
        }
    }
}
