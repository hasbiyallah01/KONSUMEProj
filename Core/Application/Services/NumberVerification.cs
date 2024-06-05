
using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DaticianProj.Core.Application.Services
{
    public class NumberVerification : INumberVerificationService
    {
        private readonly IConfiguration _configuration; 
        private readonly HttpClient _httpClient;
        public string _verificationServiceApiKey;

        public NumberVerification(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _verificationServiceApiKey = _configuration.GetSection("numlookupapi")["verificationKey"];
        }

        public async Task<BaseResponse> VerifyMobileNumber(string mobileNumber)
        {
            string requestUri = @$"https://api.numlookupapi.com/v1/validate/{mobileNumber}?apikey={_verificationServiceApiKey}&country_code=NG";
            requestUri = requestUri.Replace(" ", "");
            _httpClient.BaseAddress = new Uri(requestUri);
            HttpContent httpContent = new StringContent(JsonSerializer.Serialize(new
            {
                to = mobileNumber,
                FromApplication = "The FORT Application"
            }));
       
            var requestResponse = await _httpClient.GetAsync(requestUri);
            if(requestResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.Error.WriteLine($"Request Failed with status: {requestResponse.StatusCode}");
                return new BaseResponse
                {
                    IsSuccessful = false
                };
            }
            string responseString = await requestResponse.Content.ReadAsStringAsync();
            var parsedResponse = JsonSerializer.Deserialize<NumberVerificationResponse>(responseString);

            if (parsedResponse.Valid != true)
            {
                return new BaseResponse
                {
                    IsSuccessful = false,
                    Message = "Invalid Phone Number",
                };
            }

            return new BaseResponse
            {
                IsSuccessful = true,
            };

        }
    }
    public class NumberVerificationResponse
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("local_format")]
        public string LocalFormat { get; set; }

        [JsonPropertyName("international_format")]
        public string InternationalFormat { get; set; }

        [JsonPropertyName("country_prefix")]
        public string CountryPrefix { get; set; }

        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }

        [JsonPropertyName("country_name")]
        public string CountryName { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("carrier")]
        public string Carrier { get; set; }

        [JsonPropertyName("line_type")]
        public string LineType { get; set; }

    }
}
