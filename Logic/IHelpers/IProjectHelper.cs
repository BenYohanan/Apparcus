

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
        bool AddComments(ProjectCommentsViewModel commentDetails);
        IPagedList<ProjectViewModel> Projects(IPageListModel<ProjectViewModel> model, int page);
        IPagedList<SupporterViewModel> GetProjectSupportersPaged(int projectId, IPageListModel<SupporterViewModel> filter, int page = 1, int pageSize = 10);
        IPagedList<PaymentDTO> GetPaymentsPaged(int projectId, IPageListModel<PaymentDTO> filter, int page = 1, int pageSize = 20);
    }
}