using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? AmountNeeded { get; set; }
        public decimal? AmountObtained { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? CreatedById { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByPhoneNumber { get; set; }
        public string? CreatedByEmail { get; set; }
        public string? SupportLink { get; set; }
        public DateTime? CreatedByDateJoined { get; set; }
        public virtual ICollection<ProjectSupporter>? ProjectSupporters { get; set; }
    }
}
