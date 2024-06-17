using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models.UserModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DaticianProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("login")]
        public async Task Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse"),
                });
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            _logger.LogInformation("GoogleResponse called with returnUrl: {ReturnUrl}", returnUrl);

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            { 
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            if (!result.Succeeded)
            {
                _logger.LogError("Authentication failed.");
                return BadRequest(new { Message = "Authentication failed." });
            }

            var token = result.Properties.GetTokenValue("access_token");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Token is null or empty.");
                return BadRequest(new { Message = "Token is missing." });
            }

            var googleUser = await _userService.ValidateToken(token);
            if (googleUser == null)
            {
                _logger.LogError("Invalid token.");
                return BadRequest(new { Message = "Invalid token." });
            }

            var userRequest = new UserRequest
            {
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName,
                Email = googleUser.Email
            };

            var userResponse = await _userService.CreateUserUsingAuthAsync(token, userRequest);
            if (userResponse.IsSuccessful)
            {
                _logger.LogInformation("User created successfully: {UserMessage}", userResponse.Message);
                return Ok(new { Message = userResponse.Message, claims });
            }
            else
            {
                _logger.LogError("User creation failed: {UserMessage}", userResponse.Message);
                return BadRequest(new { Message = userResponse.Message });
            }
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logger.LogInformation("User logged out successfully.");
            return Ok(new { Message = "Successfully logged out" });
        }

        
    }
}
