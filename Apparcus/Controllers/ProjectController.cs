
//using Core.Models;
//using Logic.IHelpers;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Apparcus.Controllers
//{
//    public class ProjectController : Controller
//    {
//        private readonly IUserHelper _userHelper;
//        private readonly IProjectHelper _projectHelper;
//        private readonly ISupportersHelper _supportersHelper;
//        private readonly UserManager<ApplicationUser> _userManager;

//        public ProjectController(IUserHelper userHelper, IProjectHelper projectHelper, ISupportersHelper supportersHelper, UserManager<ApplicationUser> userManager)
//        {
//            _userHelper = userHelper;
//            _projectHelper = projectHelper;
//            _supportersHelper = supportersHelper;
//            _userManager = userManager;
//        }

//        public async Task<IActionResult> Index()
//        {
//            ViewBag.Layout = _userHelper.GetRoleLayout();
//            var projects = await _projectHelper.GetAllProjectsAsync();
//            ViewBag.Users = await _userHelper.GetAllUsersAsync();
//            return View(projects);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create(Project project, List<string> selectedUserIds)
//        {
//            if (ModelState.IsValid)
//            {
//                var currentUser = await _userManager.GetUserAsync(User);
//                project.CreatedById = currentUser?.Id;

//                project = await _projectHelper.CreateProjectAsync(project);

//                var supporters = new List<ProjectSupporter>();
//                foreach (var userId in selectedUserIds ?? new List<string>())
//                {
//                    var user = await _userHelper.GetUserByIdAsync(userId);
//                    if (user != null)
//                    {
//                        supporters.Add(new ProjectSupporter
//                        {
//                            FullName = user.FullName,
//                            Email = user.Email,
//                            PhoneNumber = user.PhoneNumber,
//                            Amount = "0" // Default, since string
//                        });
//                    }
//                }
//                await _supportersHelper.AddSupportersAsync(project.Id, supporters);

//                return RedirectToAction(nameof(Index));
//            }
//            ViewBag.Users = await _userHelper.GetAllUsersAsync();
//            return View("Index");
//        }
//    }
//}
using Core.Models;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Apparcus.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IProjectHelper _projectHelper;
        private readonly IDropdownHelper _dropdownHelper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectController(IUserHelper userHelper, IProjectHelper projectHelper, IDropdownHelper dropdownHelper, UserManager<ApplicationUser> userManager)
        {
            _userHelper = userHelper;
            _projectHelper = projectHelper;
            _dropdownHelper = dropdownHelper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Layout = _userHelper.GetRoleLayout();
            ViewBag.UsersDropdown = await _dropdownHelper.GetAllUsersDropdownAsync();

            var projects = await _projectHelper.GetAllProjectsAsync();
            return View(projects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                project.CreatedById = currentUser?.Id;

                project = await _projectHelper.CreateProjectAsync(project);

                return RedirectToAction(nameof(Index));
            }
            ViewBag.UsersDropdown = await _dropdownHelper.GetAllUsersDropdownAsync();
            return View("Index");
        }
    }
}
