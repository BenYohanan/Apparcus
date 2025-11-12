using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Core.Enums.ApparcusEnum;

namespace Core.DbContext
{
    public static class CoreSeed
    {
        public static async Task SeedDataAsync(AppDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            var dropdownTask = SeedDropdownsAsync(context);
            var roleTask = SeedRolesAsync(roleManager);
            var userTask = SeedUsersAsync(userManager);

            await Task.WhenAll(dropdownTask, roleTask, userTask);
        }

        private static async Task SeedDropdownsAsync(AppDbContext context)
        {
            var existingNames = await context.DropDowns
                .AsNoTracking()
                .Select(d => d.Name)
                .ToListAsync();

            var dropdowns = new List<DropDown>
            {
                new() { DropdownKey = DropdownEnums.Gender, Name = "Male" },
                new() { DropdownKey = DropdownEnums.Gender, Name = "Female" },
                new() { DropdownKey = DropdownEnums.Gender, Name = "Prefer not to say" }
            };

            var newDropdowns = dropdowns
                .Where(d => !existingNames.Contains(d.Name))
                .ToList();

            if (newDropdowns.Count > 0)
            {
                 await context.DropDowns.AddRangeAsync(newDropdowns);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var existingRoles = await roleManager.Roles
                .AsNoTracking()
                .Select(r => r.Name)
                .ToListAsync();

            var newRoles = SeedItems.DefaultRoles()
                .Where(r => !existingRoles.Contains(r.Name))
                .ToList();

            if (newRoles.Count == 0) return;

            var tasks = newRoles.Select(r => roleManager.CreateAsync(r));
            await Task.WhenAll(tasks);
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUsers = SeedItems.DefaultUsers();

            var existingEmails = await userManager.Users
                .AsNoTracking()
                .Select(u => u.Email)
                .ToListAsync();

            var newUsers = defaultUsers
                .Where(u => !existingEmails.Contains(u.Email))
                .ToList();

            if (newUsers.Count == 0)
                return;

            foreach (var user in newUsers)
            {
                var result = await userManager.CreateAsync(user, "11111");
                if (!result.Succeeded)
                    continue;

                string role = user.Email.ToLower() switch
                {
                    "benyohanan@apparcus.com" => SeedItems.SuperAdminRole,
                    "admin@apparcus.com" => SeedItems.AdminRole,
                    _ => SeedItems.UserRole
                };

                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

    public static class SeedItems
    {
        public const string SuperAdminRole = "SuperAdmin";
        public const string AdminRole = "Admin";
        public const string UserRole = "User";

        public const string SuperAdminId = "7B0030007800640033006600640035003000";
        public const string SystemAdminId = "30007800360034003800300030003";
        public const string SystemUserId = "723778003600312338003000495676";

        public static string SuperAdminDashboard = "/SuperAdmin/Home";
        public static string AdminDashboard = "/Admin/Index";
        public static string UserDashboard = "/User/Index";

        public static IList<IdentityRole> DefaultRoles() => new List<IdentityRole>
        {
            new IdentityRole { Name = SuperAdminRole, NormalizedName = SuperAdminRole.ToUpper(), ConcurrencyStamp = "5002C0030007800380036002C00300078" },
            new IdentityRole { Name = AdminRole, NormalizedName = AdminRole.ToUpper(), ConcurrencyStamp = "14f4bf73-9b1a-415f-9b47-626ca87f6c0e" },
            new IdentityRole { Name = UserRole, NormalizedName = UserRole.ToUpper(), ConcurrencyStamp = "0DB45C30-2FEE-47C6-AF34-7849A62B8856" }
        };

        public static IList<ApplicationUser> DefaultUsers() => new List<ApplicationUser>
        {
            new ApplicationUser
            {
                Id = SuperAdminId,
                Email = "benyohanan@apparcus.com",
                NormalizedEmail = "BENYOHANAN@APPARCUS.COM",
                NormalizedUserName = "BENYOHANAN@APPARCUS.COM",
                UserName = "benyohanan@apparcus.com",
                PasswordHash = "AQAAAAEAACcQAAAAEO3NQwqwWgetIJ/tyYRIrobEpEcvQ47xoczshXUgLyKKSuanh+CiKz//sKDMCq+PCA==",
                FirstName = "SuperAdmin",
                PhoneNumber = "0000 000 0000"
            },
            new ApplicationUser
            {
                Id = SystemAdminId,
                Email = "admin@apparcus.com",
                NormalizedEmail = "ADMIN@APPARCUS.COM",
                NormalizedUserName = "ADMIN@APPARCUS.COM",
                UserName = "admin@apparcus.com",
                PasswordHash = "AQAAAAEAACcQAAAAEO3NQwqwWgetIJ/tyYRIrobEpEcvQ47xoczshXUgLyKKSuanh+CiKz//sKDMCq+PCA==",
                FirstName = "Admin",
                PhoneNumber = "0000 000 0000"
            },
            new ApplicationUser
            {
                Id = SystemUserId,
                Email = "user@apparcus.com",
                NormalizedEmail = "USER@APPARCUS.COM",
                NormalizedUserName = "USER@APPARCUS.COM",
                UserName = "user@apparcus.com",
                PasswordHash = "AQAAAAEAACcQAAAAEO3NQwqwWgetIJ/tyYRIrobEpEcvQ47xoczshXUgLyKKSuanh+CiKz//sKDMCq+PCA==",
                FirstName = "User",
                PhoneNumber = "0000 000 0000"
            }
        };
    }
}
