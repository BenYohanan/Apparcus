using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModels
{
    public class PaymentRequestViewModel
    {
        public int ProjectId { get; set; }
        public int? ProjectSupporterId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? callbackUrl { get; set; }
        public string? Email { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaystackInitResponse {
        public bool Status { get; set; } 
        public string Message { get; set; } 
        public PaystackInitData Data { get; set; } 
    }
    public class PaystackInitData { 
        public string? Authorization_Url { get; set; } 
        public string? Access_Code { get; set; } 
        public string? Reference { get; set; } 
    }

    public class PaystackVerifyResponse {
        public bool Status { get; set; } 
        public string Message { get; set; }
        public PaystackVerifyData Data { get; set; }
    }
    public class PaystackVerifyData
    {
        public string Status { get; set; } 
        public string Reference { get; set; }
        public int Amount { get; set; }
        public PaystackCustomer Customer { get; set; }
        public PaystackMetadata Metadata { get; set; }
    }
    public class PaystackCustomer {
        public string Email { get; set; } 
    }
    public class PaystackMetadata { 
        public int? ProjectId { get; set; } 
        public string FullName { get; set; } 
        public string Phone { get; set; } 
        public int? SupporterId { get; set; } 
    }

    public class CreateRecipientRequest
    {
        public string type { get; set; } = "nuban";
        public string name { get; set; }
        public string account_number { get; set; }
        public string bank_code { get; set; }
        public string currency { get; set; } = "NGN";
    }

    public class PaystackCreateRecipientResponse 
    { 
        public bool Status { get; set; }
        public string Message { get; set; } 
        public PaystackRecipientData Data { get; set; } 
    }
    public class PaystackRecipientData 
    { 
        public string? Recipient_code { get; set; } 
        public string? Details { get; set; } 
    }

    public class CreateTransferRequest
    {
        public string? source { get; set; } = "balance";
        public decimal? amount { get; set; } 
        public string? recipient { get; set; }
        public string? reason { get; set; }
    }
    public class PaystackInitiateTransferResponse
    {
        public bool Status { get; set; }
        public string? Message { get; set; }
        public TransferData? Data { get; set; }
    }

    public class TransferData
    {
        public string? Reference { get; set; }
        public string? Status { get; set; }
    }

    public class VerifyPaymentRequest
    {
        public string? Reference { get; set; } 
    }
}
