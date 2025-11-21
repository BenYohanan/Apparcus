using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;

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
            var user = Utility.GetCurrentUser();
            var request = AppHttpContext.Current.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            var query = _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSupporters)
                .Where(p => !p.Deleted);

            if (user.UserRole == Constants.UserRole) 
            {
                query = query.Where(p => p.CreatedById == user.Id);
            }

            return await query
                .Select(c => new ProjectViewModel
                {
                    Id = c.Id,
                    Name = c.Title,
                    Description = c.Description,
                    AmountNeeded = c.AmountNeeded,
                    AmountObtained = c.AmountObtained ?? 0,
                    Deleted = c.Deleted,
                    DateCreated = c.DateCreated,
                    CreatedById = c.CreatedById,
                    CreatedBy = c.CreatedBy != null ? c.CreatedBy.FullName : "",
                    ProjectSupporters = c.ProjectSupporters.Where(x => x.Amount > 0).ToList(),
                    SupportLink = $"{baseUrl}/Guest/View/{c.Id}"
                })
                .ToListAsync();
        }

        public async Task<List<ProjectViewModel>> GetUserProjectsAsync(string userId)
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Where(p => p.CreatedById == userId && !p.Deleted)
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
                    CreatedBy = c.CreatedBy != null ? c.CreatedBy.FullName : "",
                    ProjectSupporters = c.ProjectSupporters
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

        public List<ProjectSupporter> GetContributors()
        {
            return [.. _context.ProjectSupporters.Where(ps => !ps.Deleted)];
        }

        public ProjectViewModel? GetProjectById(int id)
        {
            return _context.Projects
                .Include(p => p.CreatedBy)
                .Include(p => p.ProjectSupporters)
                .Where(p => !p.Deleted && p.Id == id)
                .Select(c => new ProjectViewModel
                {
                    Id = c.Id,
                    Name = c.Title,
                    Description = c.Description,
                    AmountNeeded = c.AmountNeeded,
                    AmountObtained = c.AmountObtained ?? 0,
                    Deleted = c.Deleted,
                    DateCreated = c.DateCreated,
                    CreatedById = c.CreatedById,
                    CreatedBy = c.CreatedBy != null ? c.CreatedBy.FullName : "",
                    CreatedByDateJoined = c.CreatedBy != null ? c.CreatedBy.DateCreated : DateTime.MinValue,
                    CreatedByEmail = c.CreatedBy != null ? c.CreatedBy.Email : "",
                    CreatedByPhoneNumber = c.CreatedBy != null ? c.CreatedBy.PhoneNumber : "",
                    ProjectSupporters = c.ProjectSupporters.Where(x=>x.Amount > 0).ToList()
                }).FirstOrDefault();
        }
    }
}