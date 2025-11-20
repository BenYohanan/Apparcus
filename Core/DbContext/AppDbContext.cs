using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DbContext
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<DropDown> DropDowns { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectSupporter> ProjectSupporters { get; set; }
        public DbSet<ProjectComment> ProjectComments { get; set; }
        public DbSet<ProjectVirtualAccount> ProjectVirtualAccounts { get; set; }
        public DbSet<ProjectBankAccount> ProjectBankAccounts { get; set; }

    }
}
