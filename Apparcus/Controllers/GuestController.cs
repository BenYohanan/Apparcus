using Microsoft.AspNetCore.Mvc;

namespace Apparcus.Controllers
{
    public class GuestController : Controller
    {
        public IActionResult View(int id)
        {
            return View();
        }
    }
}
