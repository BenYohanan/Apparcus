using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ProjectBankAccount
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public virtual Project? Project { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankCode { get; set; }
        public string? AccountName { get; set; }
        public DateTime? DateAdded { get; set; } = DateTime.UtcNow;
    }
}
