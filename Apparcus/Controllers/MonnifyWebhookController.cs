using Core.DbContext;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Apparcus.Controllers
{
    [Route("api/webhook/monnify")]
    public class MonnifyWebhookController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IMonnifyHelper _monnify;


        public MonnifyWebhookController(AppDbContext context, IMonnifyHelper monnify)
        {
            _context = context;
            _monnify = monnify;
        }


        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] dynamic payload)
        {
            try
            {
                string eventType = payload.eventType;
                if (eventType != "SUCCESSFUL_TRANSACTION") return Ok();


                string accountReference = payload.eventData.accountReference;
                decimal paidAmount = (decimal)payload.eventData.paidAmount;


                var va = await _context.ProjectVirtualAccounts.FirstOrDefaultAsync(x => x.AccountReference == accountReference);
                if (va == null) return Ok();


                var project = await _context.Projects.FindAsync(va.ProjectId);
                if (project == null) return Ok();


                // Monnify already applied splits. paidAmount is the gross amount credited into reserved account.
                project.AmountObtained = (project.AmountObtained ?? 0) + paidAmount;


                // check completion
                if (project.AmountObtained >= project.AmountNeeded)
                {
                    await _monnify.DisableReservedAccountAsync(va.AccountReference);
                    va.IsDisabled = true;
                }


                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                // log exception
                return Ok();
            }
        }
    }
}
