using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Apparcus.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IProjectHelper _projectHelper;
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserHelper userHelper, IProjectHelper projectHelper, AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userHelper = userHelper;
            _projectHelper = projectHelper;
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            var projects = await _projectHelper.GetAllProjectsAsync().ConfigureAwait(false);
            var contributors = _projectHelper.GetContributors();
            var userProjectIds = projects.Select(p => p.Id).ToHashSet();
            var contributorsCount = contributors.Count(c => userProjectIds.Contains(c.ProjectId));
            var user = Utility.GetCurrentUser();
            var userVm = new UserDashboardDto
            {
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProjectCount = projects.Count,
                Projects = projects,
                WalletBalance = _context.Wallets.Where(x => x.ProjectOwnerId == user.Id).Sum(x=>x.Balance),
                ContibutorsCount = contributorsCount
            };
            return View(userVm);
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

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");
            ViewBag.Layout = _userHelper.GetRoleLayout();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(IFormFile file, string FirstName, string LastName, string PhoneNumber)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            user.FirstName = FirstName?.Trim();
            user.LastName = LastName?.Trim();
            user.PhoneNumber = PhoneNumber?.Trim();

            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // ADD CACHE BUSTER HERE SO BROWSER DOESN'T CACHE OLD IMAGE
                user.ProfilePictureUrl = "/uploads/avatars/" + uniqueFileName + "?v=" + DateTime.Now.Ticks;
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // THIS IS THE KEY YOUR PROJECT USES EVERYWHERE
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return RedirectToAction("Profile");
        }


    }
}


