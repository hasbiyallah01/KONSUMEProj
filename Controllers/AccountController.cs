using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models.UserModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

/*namespace DaticianProj.Controllers
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
        public IActionResult Login(string returnUrl = "/")
        {
            

            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { ReturnUrl = returnUrl }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        *//*[HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            _logger.LogInformation("GoogleResponse called with returnUrl: {ReturnUrl}", returnUrl);

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

            var user = await _userService.CreateUserUsingAuthAsync(token, userRequest);
            if (user.IsSuccessful)
            {
                _logger.LogInformation("User created successfully: {UserMessage}", user.Message);
                return Ok(new { Message = user.Message });
            }
            else
            {
                _logger.LogWarning("Additional information required for Google user: {GoogleUser}", googleUser);
                return Ok(new { RequiresAdditionalInfo = true, GoogleUser = googleUser });
            }
        }*//*

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

            var user = await _userService.CreateUserUsingAuthAsync(token, userRequest);
            if (user.IsSuccessful)
            {
                _logger.LogInformation("User created successfully: {UserMessage}", user.Message);
                return Ok(new { Message = user.Message });
            }
            else
            {
                _logger.LogWarning("Additional information required for Google user: {GoogleUser}", googleUser);
                return Ok(new { RequiresAdditionalInfo = true, GoogleUser = googleUser });
            }
        }


        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] UserRequest additionalInfo)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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

            var userRequest = new UserRequest
            {
                ConfirmPassword = additionalInfo.ConfirmPassword,
                FirstName = additionalInfo.FirstName,
                LastName = additionalInfo.LastName,
                Email = additionalInfo.Email,
            };

            var user = await _userService.CreateUserUsingAuthAsync(token, userRequest);

            if (user.IsSuccessful)
            {
                _logger.LogInformation("User registration completed successfully: {UserMessage}", user.Message);
                return Ok(new { Message = user.Message });
            }
            else
            {
                _logger.LogError("User registration failed: {UserMessage}", user.Message);
                return StatusCode(400, new { Message = user.Message });
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

        [Route("users")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Value);
        }

        [Route("users/{id}")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser([FromRoute] int id)
        {
            var user = await _userService.GetUser(id);
            if (!user.IsSuccessful)
            {
                _logger.LogError("User not found: {UserId}", id);
                return NotFound(new { Message = user.Message });
            }
            var result = new JsonResult(user.Value)
            {
                StatusCode = (int?)HttpStatusCode.OK
            };
            return result;
        }

        [Route("users/{id}")]
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromForm] UserRequest request)
        {
            var user = await _userService.UpdateUser(id, request);
            if (user.IsSuccessful)
            {
                _logger.LogInformation("User updated successfully: {UserId}", id);
                return Ok(new { Message = user.Message });
            }
            _logger.LogError("User update failed: {UserMessage}", user.Message);
            return BadRequest(new { Message = user.Message });
        }

        [Route("users/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _userService.RemoveUser(id);
            if (user.IsSuccessful)
            {
                _logger.LogInformation("User deleted successfully: {UserId}", id);
                return Ok(new { Message = user.Message });
            }
            _logger.LogError("User deletion failed: {UserMessage}", user.Message);
            return BadRequest(new { Message = user.Message });
        }
    }
}*/


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
        public IActionResult Login(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { ReturnUrl = returnUrl }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            _logger.LogInformation("GoogleResponse called with returnUrl: {ReturnUrl}", returnUrl);

            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                return Ok(new { Message = userResponse.Message });
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
