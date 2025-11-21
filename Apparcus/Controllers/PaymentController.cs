using Apparcus.Models;
using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPaystackHelper _paystackHelper;
    private readonly IEmailService _email;
    private readonly IConfiguration _config;

    public PaymentController(AppDbContext context, IPaystackHelper paystackHelper, IEmailService email, IConfiguration config)
    {
        _context = context;
        _paystackHelper = paystackHelper;
        _email = email;
        _config = config;
    }

    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize([FromBody] PaymentRequestViewModel model)
    {
        if (model == null)
            return ResponseHelper.ErrorMsg();
        var projectSupporter = _context.ProjectSupporters.FirstOrDefault(x => x.Email == model.Email && !x.Deleted);
        if (projectSupporter == null)
        {
            projectSupporter = new ProjectSupporter
            {
                ProjectId = model.ProjectId,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateCreated = DateTime.Now,
                Deleted = false
            };
            _context.Add(projectSupporter);
            _context.SaveChanges();
        }
       model.callbackUrl = Url.Action("Success", "Guest", null, Request.Scheme);
        model.ProjectSupporterId = projectSupporter.Id;
        var initResponse = await _paystackHelper.InitializePayment(model);

        if (string.IsNullOrEmpty(initResponse))
            return ResponseHelper.JsonError("Unable to initialize payment");

        return ResponseHelper.JsonSuccessWithReturnUrl(initResponse);
    }


    [HttpPost("verify")]
    public async Task<JsonResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        if (string.IsNullOrEmpty(request.Reference))
            return ResponseHelper.ErrorMsg();

        if (_context.Transactions.Any(x => x.Reference == request.Reference))
            return ResponseHelper.JsonSuccess("Already processed");

        var verify = await _paystackHelper.VerifyPayment(request.Reference);
        if (verify == null || verify.Data == null || verify.Data.Status != "success")
            return ResponseHelper.ErrorMsg();

        var data = verify.Data;
        decimal amount = data.Amount / 100m; // Paystack returns amount in kobo

        var projectId = data.Metadata.ProjectId;
        var supporterId = data.Metadata.SupporterId;

        if (projectId == 0 || supporterId == 0)
            return ResponseHelper.ErrorMsg();

        var supporter = _context.ProjectSupporters.FirstOrDefault(x=>x.Id == supporterId);
        if (supporter == null)
        {
            return ResponseHelper.ErrorMsg();
        }
        supporter.Amount =+ amount;

        var contribution = new Contribution
        {
            PaystackReference = request.Reference,
            Amount = amount,
            ProjectId = projectId,
            ProjectSupporterId = supporterId,
            Date = DateTime.UtcNow
        };
        _context.Contributions.Add(contribution);

        decimal fee = 9m;
        decimal ownerGets = amount - fee;
        if (ownerGets < 0) ownerGets = 0;

        var trx = new Transaction
        {
            ProjectId = projectId,
            ProjectSupporterId = supporterId,
            AmountPaid = amount,
            PlatformFee = fee,
            ProjectOwnerReceives = ownerGets,
            Reference = request.Reference,
            Status = verify.Data.Status,
            DateCreated = DateTime.UtcNow
        };
        _context.Transactions.Add(trx);

        var project = await _context.Projects
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == projectId);
            if (project != null)
            {
                project.AmountObtained =+ amount;

                var wallet = await _context.Set<Wallet>().FirstOrDefaultAsync(w => w.ProjectOwnerId == project.CreatedBy.Id);
                if (wallet == null)
                {
                    wallet = new Wallet { ProjectOwnerId = project.CreatedBy.Id, Balance = 0m };
                    _context.Add(wallet);
                }
                wallet.Balance =+ ownerGets;

                if (project.CreatedBy?.Email != null)
                {
                    _email.SendEmail(
                        project.CreatedBy.Email,
                        "New Contribution Received!",
                        $"A supporter just contributed ₦{amount:N2} to your project <b>{project.Title}</b>."
                    );
                }
            }

        await _context.SaveChangesAsync();
        return ResponseHelper.JsonSuccess("Contribution verified!");
    }

    // Wallet endpoints
    [HttpGet("wallet/{ownerId}")]
    public async Task<IActionResult> GetWallet(string ownerId)
    {
        var wallet = await _context.Set<Wallet>().FirstOrDefaultAsync(w => w.ProjectOwnerId == ownerId);
        if (wallet == null) return NotFound();
        return Ok(wallet);
    }

    // Create recipient (store recipient code)
    [HttpPost("recipient")]
    public async Task<IActionResult> CreateRecipient([FromBody] CreateRecipientRequest req)
    {
        var resp = await _paystackHelper.CreateTransferRecipient(req);
        if (resp == null || !resp.Status) return BadRequest(resp?.Message ?? "Failed");

        return Ok(resp.Data);
    }

    // Withdraw (initiate transfer)
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        // Validate owner & balance
        var wallet = await _context.Set<Wallet>().FirstOrDefaultAsync(w => w.ProjectOwnerId == request.ProjectOwnerId);
        if (wallet == null || wallet.Balance < request.Amount)
            return BadRequest("Insufficient balance");

        // Initiate transfer to recipient code (recipient should be created earlier)
        var transferReq = new CreateTransferRequest
        {
            amount = request.Amount * 100, // kobo
            recipient = request.RecipientCode,
            reason = request.Reason
        };

        var resp = await _paystackHelper.InitiateTransfer(transferReq);
        if (resp == null || !resp.Status)
            return BadRequest(resp?.Message ?? "Transfer failed");

        // Deduct wallet immediately (or mark pending depending on business rule)
        wallet.Balance -= request.Amount;
        var withdrawal = new Withdrawal
        {
            ProjectOwnerId = request.ProjectOwnerId,
            Amount = request.Amount,
            RecipientCode = request.RecipientCode,
            TransferReference = Guid.NewGuid().ToString(), // ideally from resp data
            Status = "pending"
        };
        _context.Add(withdrawal);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Withdrawal initiated", data = resp.Data });
    }
}

public class WithdrawRequest
{
    public string ProjectOwnerId { get; set; }
    public decimal Amount { get; set; }
    public string RecipientCode { get; set; } = "";
    public string Reason { get; set; } = "Withdrawal";
}
