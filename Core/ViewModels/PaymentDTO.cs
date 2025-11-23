using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class ProjectPaymentDTO
    {
        public string? ProjectName { get; set; }
        public List<PaymentDTO>? Payments { get; set; }
    }
    public class PaymentDTO
    {
        public string? Contributor { get; set; }
        public string? Project { get; set; }
        public string? Reference { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? AmountPaid { get; set; }
    }
}
