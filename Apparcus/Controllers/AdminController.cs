using Microsoft.AspNetCore.Mvc;

namespace Apparcus.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
