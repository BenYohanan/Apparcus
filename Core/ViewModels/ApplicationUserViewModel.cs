using System.ComponentModel;
using System.Reflection;

namespace Core.ViewModels
{
    public class ApplicationUserViewModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public DateTime? DateRegistered { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsPasswordExpired { get; set; }
        public bool IsExtended { get; set; }
        public DateTime? PasswordExpiryDate { get; set; }
        public bool IsAdmin { get; set; }
       
    }
}