using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        public string? ProjectOwnerId { get; set; }
        [ForeignKey(nameof(ProjectOwnerId))]
        public virtual ApplicationUser? ProjectOwner { get; set; }
        public decimal Balance { get; set; } = 0m;
    }
}
