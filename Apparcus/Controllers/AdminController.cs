using Apparcus.Models;
using Core.DbContext;
using Core.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apparcus.Controllers
{
    public class AdminController(IUserHelper userHelper, AppDbContext appDbContext, IEmailTemplateService emailTemplateService, UserManager<ApplicationUser> userManager) : Controller
    {
        private readonly IUserHelper _userHelper = userHelper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AppDbContext _context = appDbContext;
        private readonly IEmailTemplateService _emailTemplateService = emailTemplateService;

        public IActionResult Index()
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            return View();
        }

        [HttpGet]
        public IActionResult Users()
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            var users = _userHelper.GetUsers();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new { isError = true, msg = "User not found." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
                return ResponseHelper.JsonSuccess("Password changed successfully.");

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ResponseHelper.JsonError("Failed to change password");
        }

        [HttpPost]
        public async Task<JsonResult> MakeUserAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return ResponseHelper.JsonError("User not found");

            var isAlreadyAdmin = await _userManager.IsInRoleAsync(user, SeedItems.AdminRole);
            if (isAlreadyAdmin)
                return ResponseHelper.JsonError("User is already an admin");

            var result = await _userManager.AddToRoleAsync(user, SeedItems.AdminRole);
            if (result.Succeeded)
                return ResponseHelper.JsonSuccess("User promoted to admin successfully");

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return ResponseHelper.JsonError($"Failed to promote user: {errors}");
        }

		[HttpGet]
		public async Task<IActionResult> ProjectSupporters(int id)
		{
			var project = await _context.Projects
				.Include(p => p.ProjectSupporters)
				.FirstOrDefaultAsync(p => p.Id == id);

			if (project == null) return NotFound();

			ViewBag.ProjectTitle = project.Title;
			return View(project);
		}
	}


        [HttpPost]
        public JsonResult Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return ResponseHelper.JsonError("An error has occurred, try again. Please contact support if the error persists.");

            var removeUser = _context.ApplicationUsers
                .FirstOrDefault(u => u.Id == userId && !u.Deleted);

            if (removeUser == null)
                return ResponseHelper.JsonError("User not found.");

            removeUser.Deleted = true;

            _context.SaveChanges();
            return ResponseHelper.JsonSuccess("Deleted!.");
        }

    }
}
