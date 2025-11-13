using Microsoft.AspNetCore.Mvc;

namespace Apparcus.Controllers
{
    public class ProjectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
