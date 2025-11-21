using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Withdrawal
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectOwnerId { get; set; }
        [ForeignKey(nameof(ProjectOwnerId))]
        public virtual ApplicationUser? ProjectOwner { get; set; }
        public decimal Amount { get; set; }
        public string? RecipientCode { get; set; }
        public string? TransferReference { get; set; }
        public string? Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
