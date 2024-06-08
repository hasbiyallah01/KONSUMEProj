/*using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models.UserModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DaticianProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController( IUserService userService)
        {
            _googleAuthService = googleAuthService;
            _userService = userService;
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
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded) return BadRequest();

            var token = result.Properties.GetTokenValue("access_token");
            var googleUser = await _googleAuthService.AuthenticateAsync(token);

            var userRequest = new UserRequest
            {
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName,
                Email = googleUser.Email
            };
            var user = await _userService.CreateUserUsingAuthAsync(token, userRequest);
            if (user.IsSuccessful)
            {
                return Ok(new { Message = user.Message });
            }
            else
            {
                return Ok(new { RequiresAdditionalInfo = true, GoogleUser = googleUser });
            }
        }

        [HttpPost("complete-registration")]
        public async Task<IActionResult> CompleteRegistration([FromBody] UserRequest additionalInfo)
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                return BadRequest("Authentication failed");
            }

            var token = result.Properties.GetTokenValue("access_token");

            var UserRequest = new UserRequest
            {
                ConfirmPassword = additionalInfo.ConfirmPassword,
                FirstName = additionalInfo.FirstName,
                LastName = additionalInfo.LastName,
                Email = additionalInfo.Email,
                DateOfBirth = additionalInfo.DateOfBirth,
                Gender = additionalInfo.Gender,
                Height = additionalInfo.Height,
                Password = additionalInfo.Password,
                PhoneNumber = additionalInfo.PhoneNumber,
                UserGoal = additionalInfo.UserGoal,
                Weight = additionalInfo.Weight,
            };

            var user = await _userService.CreateUserUsingAuthAsync(token, UserRequest);

            if (user.IsSuccessful)
            {
                return Ok(new { Message = user.Message });
            }
            else
            {
                return StatusCode(400, new { Message = user.Message });
            }
        }


        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
                return NotFound(user.Message);
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
                return Ok(user.Message);
            }
            return BadRequest(user.Message);
        }

        [Route("users/{id}")]
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _userService.RemoveUser(id);
            if (user.IsSuccessful)
            {
                return Ok(user.Message);
            }
            return BadRequest(user.Message);
        }
    }
}
*/

using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models.UserModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        /*[HttpGet("google-response")]
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
        }*/

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
                /*DateOfBirth = additionalInfo.DateOfBirth,
                Gender = additionalInfo.Gender,
                Height = additionalInfo.Height,
                Password = additionalInfo.Password,
                PhoneNumber = additionalInfo.PhoneNumber,
                UserGoal = additionalInfo.UserGoal,
                Weight = additionalInfo.Weight,*/
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
}
