using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Services.Account;
using System.Security.Claims;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var id))
            {
                return Unauthorized();
            }

            var user = await _service.GetByIdAsync(id);

            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpPatch("me/changepassword")]
        public async Task<ActionResult> ChangePass([FromBody] ChangePasswordRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim, out var id))
            {
                return Unauthorized();
            }

            try
            {
                await _service.ChangePasswordAsync(id, dto);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex) 
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
