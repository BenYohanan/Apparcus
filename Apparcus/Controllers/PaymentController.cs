using Core.DbContext;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apparcus.Controllers
{
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMonnifyHelper _monnify;


        public PaymentController(AppDbContext context, IMonnifyHelper monnify)
        {
            _context = context;
            _monnify = monnify;
        }


        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto dto)
        {
            var project = await _context.Projects.FindAsync(dto.ProjectId);
            if (project == null) return NotFound("Project not found");


            if ((project.AmountObtained ?? 0) < dto.Amount) return BadRequest("Insufficient funds");


            var bank = await _context.ProjectBankAccounts.FirstOrDefaultAsync(x => x.ProjectId == dto.ProjectId);
            if (bank == null) return BadRequest("No bank account linked for this project");


            // call Monnify
            var success = await _monnify.SendDisbursementAsync(bank.AccountNumber, bank.BankCode, dto.Amount);
            if (!success) return StatusCode(500, "Transfer failed");


            project.AmountObtained -= dto.Amount;
            await _context.SaveChangesAsync();


            return Ok(new { success = true, message = "Withdrawal processed" });
        }
    }
}
