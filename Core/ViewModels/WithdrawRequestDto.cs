using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class WithdrawRequestDto
    {
        public int ProjectId { get; set; }
        public decimal Amount { get; set; }
    }
}
