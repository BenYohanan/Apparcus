

using Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IProjectHelper
    {
        Task<List<Project>> GetAllProjectsAsync();
        Task<Project> CreateProjectAsync(Project project);
       
    }
}