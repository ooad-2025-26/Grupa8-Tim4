using Microsoft.AspNetCore.Mvc;

namespace BentoLab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}