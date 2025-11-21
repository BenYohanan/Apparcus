using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;

namespace Apparcus.Controllers
{
    public class GuestController(IProjectHelper projectHelper) : Controller
    {
        private readonly IProjectHelper _projectHelper = projectHelper;

        [HttpGet]
        public IActionResult View(int id)
        {
           var project = _projectHelper.GetProjectById(id);
            return View(project);
        }

        [HttpGet]
        public IActionResult Success(string reference)
        {
            if (string.IsNullOrEmpty(reference))
                return Redirect("/");

            return View();
        }
    }
}
