using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Services;
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
        private readonly IUserService _userService;

        public AccountController(IAccountService accountService, IAuthService authService, IUserService userService)
        {
            _accountService = accountService;
            _authService = authService;
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<ActionResult<User>> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
             if (!int.TryParse(userIdClaim, out var id)) return Unauthorized();
            
            var user = await _accountService.GetByIdAsync(id);
            if (user == null) return NotFound();

            try
            {
                _authService.CheckUserStatus(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new {message = ex.Message});
            }

            return Ok(user);
        }

        [HttpPatch("me/changepassword")]
        public async Task<ActionResult> ChangePass([FromBody] ChangePasswordRequest dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            if (!int.TryParse(userIdClaim, out var id)) return Unauthorized();

            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user is null) return Unauthorized();
                _authService.CheckUserStatus(user);
                await _accountService.ChangePasswordAsync(user, dto);
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
