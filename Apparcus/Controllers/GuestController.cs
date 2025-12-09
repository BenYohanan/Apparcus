using Apparcus.Models;
using Core.DbContext;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Apparcus.Controllers
{
    public class GuestController(IProjectHelper projectHelper, AppDbContext context) : Controller
    {
        private readonly IProjectHelper _projectHelper = projectHelper;
        private readonly AppDbContext _context = context;

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

        [HttpPost]
        public IActionResult Comments(string commentContents)
        {
            if (string.IsNullOrEmpty(commentContents))
            {
                return ResponseHelper.JsonError("Invalid comment details");
            }
            var comment = JsonConvert.DeserializeObject<ProjectCommentsViewModel>(commentContents);
            if (comment == null)
            {
                return ResponseHelper.JsonError("comment details could not be processed");
            }
            var isCreated = _projectHelper.AddComments(comment);
            if (isCreated)
            {
                return ResponseHelper.JsonSuccess("Comment sent Successfully");
            }
            return ResponseHelper.JsonError("Failed to create project");
        }
    }
}
