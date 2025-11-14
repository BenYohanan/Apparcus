using Core.DbContext;
using Core.Models;
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
            return project;
        }
    }
}