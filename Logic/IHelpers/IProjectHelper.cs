

using Core.Models;
using Core.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace Logic.IHelpers
{
    public interface IProjectHelper
    {
        IQueryable<ProjectViewModel> GetAllProjects();
        bool CreateProject(ProjectViewModel project);
        bool UpdateProject(ProjectViewModel project);
        List<ProjectSupporter> GetContributors();
        Task<List<ProjectViewModel>> GetUserProjectsAsync(string userId);
        ProjectViewModel? GetProjectById(int id);
        ProjectPaymentDTO GetPaymentsByProjectId(int projectId);
        IPagedList<ProjectViewModel> Projects(IPageListModel<ProjectViewModel> model, int page);
    }
}