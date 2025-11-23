using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Project
    {
        public Project()
        {
            Deleted = false;
            DateCreated = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? AmountNeeded { get; set; }
        public decimal? AmountObtained { get; set; }
        public bool Deleted { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? CreatedById { get; set; }
        [ForeignKey(nameof(CreatedById))]
        public virtual ApplicationUser? CreatedBy { get; set; }
        public virtual ICollection<ProjectSupporter>? ProjectSupporters { get; set; }
        public virtual ICollection<Contribution>? Contributions { get; set; }
        public virtual ICollection<ProjectComment>? Comments { get; set; }
        public decimal? AmountRemaining => (AmountNeeded ?? 0) - (AmountObtained ?? 0);
    }
}
