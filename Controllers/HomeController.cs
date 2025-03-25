using Microsoft.AspNetCore.Mvc;

namespace WebRestaurant.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
