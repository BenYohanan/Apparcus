using Core.DbContext;
using Core.Models;
using Logic.IHelpers;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Logic.Helpers
{
    public class MonnifyHelper : IMonnifyHelper
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;


        public MonnifyHelper(IConfiguration config, HttpClient client)
        {
            _config = config;
            _client = client;
        }

        private string BaseUrl => _config["Monnify:BaseUrl"].TrimEnd('/');
        private string ApiKey => _config["Monnify:ApiKey"];
        private string SecretKey => _config["Monnify:SecretKey"];
        private string ContractCode => _config["Monnify:ContractCode"];
        private string AdminSubAccount => _config["Monnify:AdminSubAccount"];

        public async Task<string> GetAccessTokenAsync()
        {
            var auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{ApiKey}:{SecretKey}"));
            var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/api/v1/auth/login");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);


            var res = await _client.SendAsync(request);
            res.EnsureSuccessStatusCode();


            var json = await res.Content.ReadFromJsonAsync<dynamic>();
            return json.responseBody.accessToken;
        }

        public async Task<ProjectVirtualAccount> CreateReservedAccountAsync(Project project)
        {
            var token = await GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var accountRef = $"PRJ-{project.Id}-{Guid.NewGuid():N}";


            var payload = new
            {
                accountReference = accountRef,
                accountName = project.Title,
                currencyCode = "NGN",
                contractCode = ContractCode,
                customerEmail = project.CreatedBy?.Email ?? "no-reply@yourapp.local",
                incomeSplitConfig = new[]
                {
                    new {
                        subAccountCode = AdminSubAccount,
                        feePercentage = 0,
                        feeAmount = 3,
                        splitPercentage = 0
                    }
                }
            };


            var response = await _client.PostAsJsonAsync($"{BaseUrl}/api/v2/bank-transfer/reserved-accounts", payload);
            response.EnsureSuccessStatusCode();


            var created = await response.Content.ReadFromJsonAsync<dynamic>();
            var body = created.responseBody;


            return new ProjectVirtualAccount
            {
                ProjectId = project.Id,
                AccountReference = accountRef,
                ReservationReference = (string)body.reservationReference,
                AccountNumber = (string)body.accountNumber,
                BankName = (string)body.bankName,
                IsDisabled = false
            };
        }

        public async Task DisableReservedAccountAsync(string accountReference)
        {
            var token = await GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var response = await _client.PostAsync($"{BaseUrl}/api/v2/bank-transfer/reserved-accounts/{accountReference}/disable", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> SendDisbursementAsync(string accountNumber, string bankCode, decimal amount, string narration = "Project withdrawal")
        {
            var token = await GetAccessTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var payload = new
            {
                amount = amount,
                reference = Guid.NewGuid().ToString(),
                narration = narration,
                destinationBankCode = bankCode,
                destinationAccountNumber = accountNumber
            };


            var res = await _client.PostAsJsonAsync($"{BaseUrl}/api/v2/disbursements/single", payload);
            return res.IsSuccessStatusCode;
        }
    }
}
