using Core.DbContext;
using Core.Models;
using Logic.IHelpers;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
    public class ProjectHelper : IProjectHelper
    {
        private readonly AppDbContext _context;
        private readonly IMonnifyHelper _monnifyHelper;

        public ProjectHelper(AppDbContext context, IMonnifyHelper monnifyHelper)
        {
            _context = context;
            _monnifyHelper = monnifyHelper;
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _context.Projects
                .Include(p => p.CreatedBy)
                .Where(p => !p.Deleted)
                .ToListAsync();
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var va = await _monnifyHelper.CreateReservedAccountAsync(project);
            _context.ProjectVirtualAccounts.Add(va);
            return project;
        }
    }
}