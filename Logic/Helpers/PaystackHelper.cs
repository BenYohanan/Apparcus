using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Logic.Helpers 
{
    public class PaystackHelper : IPaystackHelper
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public PaystackHelper(IConfiguration config, HttpClient? client = null)
        {
            _config = config;
            _client = client ?? new HttpClient();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["Paystack:SecretKey"]);
        }

        public async Task<string> InitializePayment(PaymentRequestViewModel model)
        {
            var payload = new
            {
                email = model.Email,
                amount = (int)(model.Amount * 100),
                callback_url = model.callbackUrl,
                metadata = new
                {
                    projectId = model.ProjectId,
                    fullName = model.FullName,
                    phone = model.PhoneNumber,
                    supporterId = model.ProjectSupporterId
                }
            };

            var response = await _client.PostAsJsonAsync("https://api.paystack.co/transaction/initialize", payload);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<PaystackInitResponse>();
            return result?.Data?.Authorization_Url ?? "";
        }

        public async Task<PaystackVerifyResponse?> VerifyPayment(string reference)
        {
            var response = await _client.GetFromJsonAsync<PaystackVerifyResponse>(
                $"https://api.paystack.co/transaction/verify/{reference}");
            return response;
        }

        public async Task<PaystackCreateRecipientResponse?> CreateTransferRecipient(CreateRecipientRequest req)
        {
            var response = await _client.PostAsJsonAsync("https://api.paystack.co/transferrecipient", req);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaystackCreateRecipientResponse>();
        }

        public async Task<PaystackInitiateTransferResponse?> InitiateTransfer(CreateTransferRequest req)
        {
            var response = await _client.PostAsJsonAsync("https://api.paystack.co/transfer", req);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PaystackInitiateTransferResponse>();
        }
    }

}
