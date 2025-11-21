using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class SupportProjectViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public string? ProjectDescription { get; set; }
        public decimal AmountNeeded { get; set; }
        public decimal AmountObtained { get; set; }
        public decimal AmountRemaining => AmountNeeded - AmountObtained;
        public int SupportersCount { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Your Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter amount")]
        [Range(1, 10000000, ErrorMessage = "Amount must be greater than zero")]
        public string Amount { get; set; } = string.Empty;

        [Phone]
        [Display(Name = "Phone Number (Optional)")]
        public string? PhoneNumber { get; set; }
    }
}

