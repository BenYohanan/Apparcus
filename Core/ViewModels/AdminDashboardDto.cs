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
        public int? TotalEarnings { get; set; }
        public string? UserName { get; set; }
        public List<ProjectViewModel>? Projects { get; set; }
    }
}
