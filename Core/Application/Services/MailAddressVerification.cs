
using System.Text.Json;
using System.Text.Json.Serialization;
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models;

namespace DaticianProj.Core.Application.Services
{
    public class MailAddressVerification:IMailAddressVerification
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        public string _verificationServiceApiKey;

        public MailAddressVerification(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _verificationServiceApiKey = _configuration.GetSection("abstractApi")["verificationKey"];
        }

        public async Task<BaseResponse> VerifyMailAddress(string emailAddress)
        {
            string requestUri = @$"https://emailvalidation.abstractapi.com/v1/?api_key={_verificationServiceApiKey}&email={emailAddress}";
            requestUri = requestUri.Replace(" ", "");
            _httpClient.BaseAddress = new Uri(requestUri);
            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(new
            {
                to = emailAddress,
                FromApplication = "The KONSUME Application"
            }));

            var requestResponse = await _httpClient.GetAsync(requestUri);
            if (requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.Error.WriteLine($"Request Failed with status: {requestResponse.StatusCode}");
                return new BaseResponse
                {
                    IsSuccessful = false
                };
            }
            string responseString = await requestResponse.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<MailAddressVerificationResponse>(responseString);

            if (parsedResponse.Deliverability != "DELIVERABLE")
            {
                return new BaseResponse
                {
                    IsSuccessful = false,
                    Message = "Invalid EmailAddress",
                };
            }

            return new BaseResponse
            {
                IsSuccessful = true,
            };
        }

    }

    public class MailAddressVerificationResponse
    {
        [JsonPropertyName ("email")]
        public string EmailAddress { get; set; }
        [JsonPropertyName("auto_correct")]
        public string AutoCorrect { get; set; }
        [JsonPropertyName("deliverability")]
        public string Deliverability { get; set; }

       /* [JsonPropertyName("quality_score")]
        public d QualityScore { get; set; }*/

      

        //[JsonPropertyName("is_free_email")]
        //public bool IsFreeEmail { get; set; }

        //[JsonPropertyName("is_disposable_email")]
        //public bool IsDisposableEmail { get; set; }

        //[JsonPropertyName("is_role_email")]
        //public bool IsRoleEmail { get; set; }

        //[JsonPropertyName("is_catchall_email")]
        //public bool IsCatchAllEmail { get; set; }

        //[JsonPropertyName("is_mx_found")]
        //public bool IsMxFound { get; set; }

        //[JsonPropertyName("is_smtp_valid")]
        //public bool IsSmtpValid { get; set; }
    }
}

