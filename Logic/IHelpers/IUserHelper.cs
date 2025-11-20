using Core.Models;
using Core.ViewModels;

namespace Logic.IHelpers
{
    public interface IUserHelper
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByUserNameAsync(string userName);
        string GetRoleLayout();
        List<ApplicationUserViewModel> GetUsers();
        string GetValidatedUrl(List<string> roles);
        Task<ApplicationUser?> RegisterUser(ApplicationUserViewModel applicationUserViewModel);
        string GetCurrentUserId();
        Task<ApplicationUser?> CreateUserFromAdmin(ApplicationUserViewModel model);

    }
}
