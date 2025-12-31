using Apparcus.Models;
using Core.DbContext;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

[Route("api/payment")]
[ApiController]
public class PaymentController(AppDbContext context, IPaystackHelper paystackHelper, IEmailService email) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IPaystackHelper _paystackHelper = paystackHelper;
    private readonly IEmailService _email = email;

    // --------------------------------------------------------
    // INIT PAYMENT
    // --------------------------------------------------------
    [HttpPost("initialize")]
    public async Task<IActionResult> Initialize([FromBody] PaymentRequestViewModel model)
    {
        if (model == null)
            return ResponseHelper.ErrorMsg();

        var supporter = _context.ProjectSupporters
            .FirstOrDefault(x => x.PhoneNumber == model.PhoneNumber && !x.Deleted && x.ProjectId == model.ProjectId);

        if (supporter == null)
        {
            supporter = new ProjectSupporter
            {
                ProjectId = model.ProjectId,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateCreated = DateTime.UtcNow
            };
            _context.ProjectSupporters.Add(supporter);
            _context.SaveChanges();
        }

        model.callbackUrl = Url.Action("Success", "Guest", null, Request.Scheme);
        model.ProjectSupporterId = supporter.Id;

        var response = await _paystackHelper.InitializePayment(model);
        if (string.IsNullOrEmpty(response))
            return ResponseHelper.JsonError("Unable to initialize payment");

        return ResponseHelper.JsonSuccessWithReturnUrl(response);
    }

    // --------------------------------------------------------
    // VERIFY PAYMENT
    // --------------------------------------------------------
    [HttpPost("verify")]
    public async Task<JsonResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
    {
        if (string.IsNullOrEmpty(request.Reference))
            return ResponseHelper.ErrorMsg();

        if (_context.Transactions.Any(x => x.Reference == request.Reference))
            return ResponseHelper.JsonSuccess("Already processed");

        var verify = await _paystackHelper.VerifyPayment(request.Reference);
        if (verify == null || verify.Data?.Status != "success")
            return ResponseHelper.ErrorMsg();

        var data = verify.Data;
        decimal amount = data.Amount / 100m;

        var projectId = data.Metadata.ProjectId;
        var supporterId = data.Metadata.SupporterId;

        if (projectId == 0 || supporterId == 0)
            return ResponseHelper.ErrorMsg();

        var supporter = await _context.ProjectSupporters.FindAsync(supporterId);
        if (supporter == null)
            return ResponseHelper.ErrorMsg();

        supporter.Amount =+ amount;

        _context.Contributions.Add(new Contribution
        {
            Amount = amount,
            PaystackReference = request.Reference,
            ProjectId = projectId,
            ProjectSupporterId = supporterId,
            Date = DateTime.UtcNow
        });

        if (data.Metadata.CustomFields != null && data.Metadata.CustomFields.Any())
        {
            var projectData = data.Metadata;
            foreach (var field in data.Metadata.CustomFields)
            {
                _context.ProjectCustomFieldValues.Add(new ProjectCustomFieldValue
                {
                    ProjectId = projectData.ProjectId,
                    ProjectSupporterId = projectData.SupporterId,
                    ProjectCustomFieldId = field.ProjectCustomFieldId.Value,
                    Value = field.Value
                });
            }
        }

        decimal fee = 9m;
        decimal ownerGets = Math.Max(0, amount - fee);

        _context.Transactions.Add(new Transaction
        {
            ProjectId = projectId,
            ProjectSupporterId = supporterId,
            AmountPaid = amount,
            PlatformFee = fee,
            ProjectOwnerReceives = ownerGets,
            Reference = request.Reference,
            Status = verify.Data.Status,
            DateCreated = DateTime.UtcNow
        });

        var project = await _context.Projects
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == projectId);

        if (project != null)
        {
            project.AmountObtained =+ amount;

            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.ProjectId == project.Id);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    ProjectId = project.Id,
                    Balance = 0m,
                    ProjectOwnerId = project.CreatedById,
                };
                _context.Wallets.Add(wallet);
            }

            wallet.Balance =+ ownerGets;

            if (!string.IsNullOrEmpty(project.CreatedBy.Email))
            {
                _email.SendEmail(
                    project.CreatedBy.Email,
                    "New Contribution Received!",
                    $"You received ₦{amount:N2} on project <b>{project.Title}</b>.");
            }
        }

        await _context.SaveChangesAsync();
        return ResponseHelper.JsonSuccess("Contribution verified!");
    }

    // --------------------------------------------------------
    // CREATE RECIPIENT
    // --------------------------------------------------------
    [HttpPost("recipient")]
    public async Task<IActionResult> CreateRecipient([FromBody] CreateRecipientRequest req)
    {
        var resp = await _paystackHelper.CreateTransferRecipient(req);
        if (resp == null || !resp.Status)
            return BadRequest(resp?.Message);

        return Ok(resp.Data);
    }

    // --------------------------------------------------------
    // WITHDRAW
    // --------------------------------------------------------
    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequest req)
    {
        var project = await _context.Projects
            .FirstOrDefaultAsync(x => x.Id == req.ProjectId);

        if (project == null)
            return BadRequest("Project not found");

        if (project.CreatedById != req.ProjectOwnerId)
            return BadRequest("Unauthorized withdrawal");

        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.ProjectId == req.ProjectId);

        if (wallet == null)
            return BadRequest("Wallet not found");

        if (wallet.Balance < req.Amount)
            return BadRequest("Insufficient balance");

        var transferReq = new CreateTransferRequest
        {
            amount = req.Amount * 100,
            recipient = req.RecipientCode,
            reason = req.Reason
        };

        var resp = await _paystackHelper.InitiateTransfer(transferReq);
        if (resp == null || !resp.Status)
            return BadRequest(resp.Message);

        wallet.Balance =- req.Amount;

        var withdrawal = new Withdrawal
        {
            ProjectId = req.ProjectId,
            Amount = req.Amount,
            RecipientCode = req.RecipientCode,
            TransferReference = resp.Data.Reference,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Withdrawals.Add(withdrawal);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            message = "Withdrawal initiated",
            reference = resp.Data.Reference
        });
    }
}

public class WithdrawRequest
{
    public string ProjectOwnerId { get; set; }
    public int ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string RecipientCode { get; set; }
    public string Reason { get; set; } = "Withdrawal";
}
