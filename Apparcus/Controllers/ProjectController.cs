using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apparcus.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IProjectHelper _projectHelper;
        private readonly IDropdownHelper _dropdownHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;

        public ProjectController(IUserHelper userHelper, IProjectHelper projectHelper, IDropdownHelper dropdownHelper, UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userHelper = userHelper;
            _projectHelper = projectHelper;
            _dropdownHelper = dropdownHelper;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            ViewBag.UsersDropdown = await _dropdownHelper.GetAllUsersDropdownAsync();

            var projects = await _projectHelper.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpPost]
        public IActionResult Create(string projectDetails)
        {
            if (string.IsNullOrEmpty(projectDetails))
            {
                return Json(new { success = false, message = "Invalid project details" });
            }
            var project = System.Text.Json.JsonSerializer.Deserialize<ProjectViewModel>(projectDetails);
            if (project == null)
            {
                return Json(new { success = false, message = "Project details could not be processed" });
            }
            var existingProject = _context.Projects
                .FirstOrDefault(p => p.Title == project.Name && !p.Deleted);
            if (existingProject != null)
            {
                return Json(new { success = false, message = "Project with similar name exists" });
            }

            var isCreated = _projectHelper.CreateProject(project);
            if (isCreated)
            {
                return Json(new { success = true, message = "Project created successfully" });
            }
            return Json(new { success = false, message = "Failed to create project" });
        }

        [HttpGet]
        public IActionResult GetProjectById(int id)
        {
            var project = _context.Projects
                .Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ProjectViewModel
                {
                    Id = x.Id,
                    Name = x.Title,
                    Description = x.Description,
                    AmountNeeded = x.AmountNeeded
                })
                .FirstOrDefault();

            if (project == null)
                return Json(null);

            return Json(project);
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


        [HttpPost]
        public IActionResult Edit(string projectDetails)
        {
            if (string.IsNullOrEmpty(projectDetails))
                return Json(new { success = false, message = "Invalid project details" });

            var project = System.Text.Json.JsonSerializer.Deserialize<ProjectViewModel>(projectDetails);
            if (project == null)
                return Json(new { success = false, message = "Project data could not be processed" });

            var updated = _projectHelper.UpdateProject(project);
            if (updated)
                return Json(new { success = true, message = "Project updated successfully" });

            return Json(new { success = false, message = "Failed to update project" });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id && !p.Deleted);
            if (project == null)
                return Json(new { success = false, message = "Project not found" });

            project.Deleted = true; 
            _context.SaveChanges();

            return Json(new { success = true, message = "Project deleted successfully" });
        }


    }
}
