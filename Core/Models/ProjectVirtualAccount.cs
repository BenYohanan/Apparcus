using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ProjectVirtualAccount
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public virtual Project? Project { get; set; }
        public string? AccountReference { get; set; }
        public string? ReservationReference { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.UtcNow;
    }

}
