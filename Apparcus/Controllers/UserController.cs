using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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

            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null) return NotFound();

            var userVm = new ApplicationUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                DateRegistered = user.DateCreated,
              
            };
            var numberOfProject = await _context.Projects
                 .Where(p => p.CreatedById == userId && !p.Deleted)
                 .CountAsync();
            var supporterRows = await _context.ProjectSupporters
                .Where(s => s.Email == user.Email && !s.Deleted)
                .ToListAsync();
            var validSupporters = supporterRows
                .Where(s => decimal.TryParse(s.Amount, out _))
                .ToList();
            ViewBag.TotalProjects = numberOfProject;

            ViewBag.TotalSupported = validSupporters.Count;

            ViewBag.TotalRaised = validSupporters
                .Sum(s => decimal.Parse(s.Amount ?? "0"));

            return View(userVm);
        }
        public async Task<IActionResult> MyProject()
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


