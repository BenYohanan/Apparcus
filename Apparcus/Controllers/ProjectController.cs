using Apparcus.Models;
using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic;
using Logic.Helpers;
using Logic.IHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index(IPageListModel<ProjectViewModel> model, int page = 1)
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            ViewBag.UsersDropdown = await _dropdownHelper.GetAllUsersDropdownAsync();
            var projects =  _projectHelper.Projects(model, page);
            model.Model = projects;
            model.SearchAction = "Index";
            model.SearchController = "Project";
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(string projectDetails)
        {
            if (string.IsNullOrEmpty(projectDetails))
            {
                return ResponseHelper.JsonError("Invalid project details");
            }
            var project = System.Text.Json.JsonSerializer.Deserialize<ProjectViewModel>(projectDetails);
            if (project == null)
            {
                return ResponseHelper.JsonError("Project details could not be processed");
            }
            var existingProject = _context.Projects
                .FirstOrDefault(p => p.Title == project.Name && !p.Deleted);
            if (existingProject != null)
            {
                return ResponseHelper.JsonError("Project with name exists");
            }

            var isCreated = _projectHelper.CreateProject(project);
            if (isCreated)
            {
                return ResponseHelper.JsonSuccess("Project created successfully");
            }
            return ResponseHelper.JsonError("Failed to create project");
        }

        [HttpGet]
        public IActionResult GetProjectById(int id)
        {
            var request = AppHttpContext.Current.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";
            var project = _context.Projects
                .Where(x => x.Id == id && !x.Deleted)
                .Select(x => new ProjectViewModel
                {
                    Id = x.Id,
                    Name = x.Title,
                    Description = x.Description,
                    AmountNeeded = x.AmountNeeded,
                    SupportLink = $"{baseUrl}/Guest/View/{x.Id}"
                })
                .FirstOrDefault();

            if (project == null)
                return Json(null);

            var qrBytes = ProjectHelper.GenerateQRCode(project.SupportLink);
            if (qrBytes != null)
            {
                project.QRCodeBase64 = Convert.ToBase64String(qrBytes);
            }

            return Json(project);
        }


        [HttpGet]
        public IActionResult Supporters(int projectId, IPageListModel<SupporterViewModel> model, int page = 1)
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();

            // Preserve projectId in pagination
            ViewData["projectId"] = projectId;

            // Setup search
            model.SearchAction = "Supporters";
            model.SearchController = "Project";
            model.CanFilterByDateRange = true;

            var supporters = _projectHelper.GetProjectSupportersPaged(projectId, model, page);
            model.Model = supporters;

            var projectTitle = _context.Projects
                .Where(p => p.Id == projectId && !p.Deleted)
                .Select(p => p.Title)
                .FirstOrDefault() ?? "Project Supporters";

            ViewBag.ProjectTitle = projectTitle;

            return View(model);
        }


        [HttpGet]
        public IActionResult Payments(int projectId, IPageListModel<PaymentDTO> model, int page = 1)
        {
            if (projectId <= 0) return NotFound();

            ViewBag.Layout = _userHelper.GetRoleLayout();

            model.SearchAction = "Payments";
            model.SearchController = "Project";
            model.CanFilterByDateRange = true;

            var payments = _projectHelper.GetPaymentsPaged(projectId, model, page);
            model.Model = payments;

            ViewBag.ProjectName = _context.Projects
                .Where(p => p.Id == projectId && !p.Deleted)
                .Select(p => p.Title)
                .FirstOrDefault() ?? "Project Payments";

            return View(model);
        }



        [HttpPost]
        public IActionResult Edit(string projectDetails)
        {
            if (string.IsNullOrEmpty(projectDetails))
                return ResponseHelper.JsonError("Invalid project details");

            var project = System.Text.Json.JsonSerializer.Deserialize<ProjectViewModel>(projectDetails);
            if (project == null)
                return ResponseHelper.JsonError("Project data could not be processed");

            var updated = _projectHelper.UpdateProject(project);
            if (updated)
                return ResponseHelper.JsonSuccess("Project updated successfully");

            return ResponseHelper.JsonError("Failed to update project");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var project = _context.Projects.FirstOrDefault(p => p.Id == id && !p.Deleted);
            if (project == null)
                return ResponseHelper.JsonError("Project not found");

            project.Deleted = true;
            _context.SaveChanges();

            return ResponseHelper.JsonSuccess("Project deleted successfully");
        }

        public IActionResult Payments(int projectId)
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            var payments = _projectHelper.GetPaymentsByProjectId(projectId);
            return View(payments);
        }

    }
}