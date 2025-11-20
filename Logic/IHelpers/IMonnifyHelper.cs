using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IMonnifyHelper
    {
        Task<string> GetAccessTokenAsync();
        Task<ProjectVirtualAccount> CreateReservedAccountAsync(Project project);
        Task DisableReservedAccountAsync(string accountReference);
        Task<bool> SendDisbursementAsync(string accountNumber, string bankCode, decimal amount, string narration = "Project withdrawal");
    }

}
