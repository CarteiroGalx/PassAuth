using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Services.Interfaces;
using System.Security.Claims;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;

        public AccountController(IAccountService accountService, IAuthService authService)
        {
            _accountService = accountService;
            _authService = authService;
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
            var user = await _accountService.GetByIdAsync(id);
            if (user == null) return NotFound();

            try
            {
                await _authService.ValidateAcess(user);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new {message = "Sua conta está banida, suspensa ou desativada."});
            }

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
                await _accountService.ChangePasswordAsync(id, dto);
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
