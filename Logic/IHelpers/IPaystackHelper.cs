using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.IHelpers
{
    public interface IPaystackHelper
    {
        Task<string> InitializePayment(PaymentRequestViewModel model);
        Task<PaystackVerifyResponse?> VerifyPayment(string reference);
        Task<PaystackCreateRecipientResponse?> CreateTransferRecipient(CreateRecipientRequest req);
        Task<PaystackInitiateTransferResponse?> InitiateTransfer(CreateTransferRequest req);
    }

}
