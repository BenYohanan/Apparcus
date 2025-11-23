using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Contribution
    {
        [Key]
        public int Id { get; set; }
        public string? PaystackReference { get; set; }
        public int? ProjectId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public virtual Project? Project { get; set; }
        public decimal Amount { get; set; }
        public int? ProjectSupporterId { get; set; }
        [ForeignKey(nameof(ProjectSupporterId))]
        public virtual ProjectSupporter? ProjectSupporter { get; set; }
        public DateTime? Date { get; set; }
    }

}
