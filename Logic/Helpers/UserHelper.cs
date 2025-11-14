using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Logic.Helpers
{
	public class UserHelper(AppDbContext db, UserManager<ApplicationUser> userManager) : IUserHelper
	{
		private readonly AppDbContext db = db;
		private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<ApplicationUser?> FindByEmailAsync(string email)
		{
			return await db.ApplicationUsers
				.Where(s => s.Email == email && !s.Deleted)
				.FirstOrDefaultAsync().ConfigureAwait(false);
		}
        public async Task<ApplicationUser?> FindByUserNameAsync(string userName)
        {
            return await db.ApplicationUsers
                .Where(s => s.UserName == userName && !s.Deleted)
                .FirstOrDefaultAsync().ConfigureAwait(false);
        }
        public string GetValidatedUrl(List<string> roles)
		{
			var roleUrlMap = new Dictionary<string, string>
			{
				{ SeedItems.SuperAdminRole, SeedItems.SuperAdminDashboard },
				{ SeedItems.AdminRole, SeedItems.AdminDashboard },
				{ SeedItems.UserRole, SeedItems.UserDashboard }
			};

			foreach (var role in roles)
			{
				if (roleUrlMap.TryGetValue(role, out var url))
				{
					return url;
				}
			}

			return "/Account/Login";

		}
        public async Task<ApplicationUser?> RegisterUser(ApplicationUserViewModel applicationUserViewModel)
        {
            var user = new ApplicationUser
            {
                FirstName = applicationUserViewModel.FirstName,
                LastName = applicationUserViewModel.LastName,
                Email = applicationUserViewModel.Email,
                PhoneNumber = applicationUserViewModel.PhoneNumber,
                UserName = applicationUserViewModel.Email,
                DateOfBirth = applicationUserViewModel.DateOfBirth
            };

            var addedUser = await _userManager.CreateAsync(user, applicationUserViewModel.Password).ConfigureAwait(false);
            if (addedUser.Succeeded)
            {
                var addedUserToRole = await _userManager.AddToRoleAsync(user, SeedItems.UserRole).ConfigureAwait(false);
                if (addedUserToRole.Succeeded)
                {
                    return user;
                }
            }
            return null;
        }
        public List<ApplicationUserViewModel> GetUsers()
        {
            var adminUsers = _userManager.GetUsersInRoleAsync(SeedItems.AdminRole).Result;
            var normalUsers = _userManager.GetUsersInRoleAsync(SeedItems.UserRole).Result;

            var allUsers = adminUsers.Concat(normalUsers).DistinctBy(u => u.Id).Where(x=>!x.Deleted).ToList();

            return [.. allUsers.Select(r =>
            {
                bool isAdmin = adminUsers.Any(a => a.Id == r.Id);

                return new ApplicationUserViewModel
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    Email = r.Email,
                    FullName = $"{r.FirstName} {r.LastName}",
                    PhoneNumber = r.PhoneNumber,
                    DateRegistered = r.DateCreated,
                    DateOfBirth = r.DateOfBirth,
                    IsAdmin = isAdmin
                };
            })];
        }
        public string GetRoleLayout()
        {
            var user = Utility.GetCurrentUser();
            if (user == null)
            {
                return Constants.DefaultLayout;
            }
            var isSuperAdmin = user.Roles.Contains(Constants.SuperAdminRole);
            return isSuperAdmin ? Constants.SuperAdminLayout : Constants.GeneralLayout;
        }

    }
}