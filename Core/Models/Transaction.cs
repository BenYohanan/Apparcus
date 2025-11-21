using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public virtual Project? Project { get; set; }
        public int? ProjectSupporterId { get; set; }
        [ForeignKey(nameof(ProjectSupporterId))]
        public virtual ProjectSupporter? ProjectSupporter { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? PlatformFee { get; set; } = 9;
        public decimal? ProjectOwnerReceives { get; set; }
        public string? Reference { get; set; }
        public string? Status { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }

}
