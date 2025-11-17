using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.Helpers
{
    public class ProjectHelper : IProjectHelper
    {
        private readonly AppDbContext _context;

        public ProjectHelper(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectViewModel>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Where(p => !p.Deleted)
                .Select(c => new ProjectViewModel
                {
                    Id = c.Id,
                    Name = c.Title,
                    Description = c.Description,
                    AmountNeeded = c.AmountNeeded,
                    AmountObtained = c.AmountObtained,
                    Deleted = c.Deleted,
                    DateCreated = c.DateCreated,
                    CreatedById = c.CreatedById,
                    CreatedBy = c.CreatedBy != null ? c.CreatedBy.FullName : ""
                })
                .ToListAsync();
        }

        public bool CreateProject(ProjectViewModel project)
        {
            var createdById = Utility.GetCurrentUser().Id;
            var newProject = new Project
            {
                Title = project.Name,
                Description = project.Description,
                AmountNeeded = project.AmountNeeded,
                CreatedById = createdById
            };

            _context.Add(newProject);
            _context.SaveChanges();
            return true;
        }

        public bool UpdateProject(ProjectViewModel project)
        {
            var projectVM = _context.Projects
                .FirstOrDefault(p => p.Id == project.Id && !p.Deleted);

            if (projectVM == null)
                return false;

            projectVM.Title = project.Name;
            projectVM.Description = project.Description;
            projectVM.AmountNeeded = project.AmountNeeded;

            _context.SaveChanges();
            return true;
        }


    }
}