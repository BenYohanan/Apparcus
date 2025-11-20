using Core.Models;
using Core.ViewModels;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Apparcus.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IProjectHelper _projectHelper;

        public UserController(IUserHelper userHelper, IProjectHelper projectHelper)
        {
            _userHelper = userHelper;
            _projectHelper = projectHelper;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            var userId = _userHelper.GetCurrentUserId();

            var projects = await _projectHelper.GetUserProjectsAsync(userId);
            return View(projects);
        }

        [HttpPost]
        public IActionResult CreateFromAdmin(string userDetails)
        {
            if (string.IsNullOrEmpty(userDetails))
                return Json(new { success = false, message = "Invalid user details" });

            var model = JsonSerializer.Deserialize<ApplicationUserViewModel>(userDetails);
            if (model == null)
                return Json(new { success = false, message = "User details could not be processed" });

            var existingUser = _userHelper.FindByEmailAsync(model.Email).Result;
            if (existingUser != null)
                return Json(new { success = false, message = "Email already exists" });

            var createdUser = _userHelper.CreateUserFromAdmin(model).Result;
            if (createdUser != null)
                return Json(new { success = true, message = "User created successfully" });

            return Json(new { success = false, message = "Failed to create user" });
        }
    }
}


