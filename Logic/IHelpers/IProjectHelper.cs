

using Core.Models;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IProjectHelper
    {
        Task<List<ProjectViewModel>> GetAllProjectsAsync();
        bool CreateProject(ProjectViewModel project);
        bool UpdateProject(ProjectViewModel project);
        List<ProjectSupporter> GetContributors();
        Task<List<ProjectViewModel>> GetUserProjectsAsync(string userId);
    }
}