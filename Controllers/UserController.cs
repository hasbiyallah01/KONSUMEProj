using DaticianProj.Core.Application.Interfaces.Services;
using DaticianProj.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace KonsumeTestRun.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users.Value);
        }

        [HttpGet("{id}")]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
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


