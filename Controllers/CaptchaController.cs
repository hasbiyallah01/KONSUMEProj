using Microsoft.AspNetCore.Mvc;

namespace DaticianProj.Controllers
{
    public class CaptchaController : ControllerBase
    {

        [HttpPost]
        public async Task<ActionResult> YourSignupEndpoint(string gRecaptchaResponse)
        {
            string secretKey = "6LejQacnAAAAAHdPPHqycAKXa6L_tsnX9guHZ27H";

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={gRecaptchaResponse}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

                if (data.success == "true")
                {
                    return Content("Signup successful!");
                }
                else
                {
                    return Content("CAPTCHA verification failed.");
                }
            }
        }
    }

}

