using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Apparcus.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserHelper _userHelper;

        public UserController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Index()
        {
            return View();
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


