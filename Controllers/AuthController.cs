using PassAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PassAuth.Context;
using PassAuth.Models.Enums;
using PassAuth.DTOs.User;
using PassAuth.Services.Auth;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterRequest request)
        {
            try
            {
                var response = await _service.Register(request);
                return Ok(response);
            }
            catch(InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message  });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest request)
        {
            try
            {
                var response = await _service.Login(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
