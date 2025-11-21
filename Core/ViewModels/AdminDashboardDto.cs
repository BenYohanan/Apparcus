using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class AdminDashboardDto
    {
        public int? ProjectCount { get; set; }
        public int? ClientCount { get; set; }
        public int? ContibutorsCount { get; set; }
        public decimal? TotalEarnings { get; set; }
        public string? UserName { get; set; }
        public List<ProjectViewModel>? Projects { get; set; }
    }
    public class UserDashboardDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? ProjectCount { get; set; }
        public int? ContibutorsCount { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? WalletBalance { get; set; }
        public List<ProjectViewModel>? Projects { get; set; }
    }
}
