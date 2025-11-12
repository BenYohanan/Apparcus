using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IUserHelper
    {
        Task<ApplicationUser?> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindByUserNameAsync(string userName);
        List<ApplicationUserViewModel> GetUsers();
        string GetValidatedUrl(List<string> roles);
        Task<ApplicationUser?> RegisterUser(ApplicationUserViewModel applicationUserViewModel);
    }
}
