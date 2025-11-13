using Microsoft.AspNetCore.Mvc;

namespace Apparcus.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
