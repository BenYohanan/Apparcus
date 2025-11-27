using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Deleted = false;
            DateCreated = DateTime.UtcNow;
        }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool Deleted { get; set; }
        public DateTime DateCreated { get; set; }
        [NotMapped]
        public string FullName => FirstName + " " + LastName;
        [NotMapped]
        public List<string?> Roles { get; set; }
        [NotMapped]
        public string? UserRole { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
