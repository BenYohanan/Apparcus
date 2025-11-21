using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class SupporterViewModel
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public decimal AmountNeeded { get; set; }
        public decimal AmountObtained { get; set; }
        public decimal AmountRemaining => AmountNeeded - AmountObtained;
        public int SupportersCount { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}

