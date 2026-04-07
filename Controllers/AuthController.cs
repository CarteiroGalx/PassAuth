using PassAuth.DTOs;
using PassAuth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PassAuth.Context;
using PassAuth.Models.Enums;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserDto request)
        {
            var newUser = new User
            {
                Username = request.Username,
                Role = UserRole.User
            };
            var hasher = new PasswordHasher<User>();
            newUser.PasswordHash = hasher.HashPassword(newUser, request.Password);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            var response = new
            {
                id = newUser.Id,
                username = newUser.Username,
                response = "Usuário resgistrado com sucesso!"
            };

            return StatusCode(201, response);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto request)
        {
           if (user.Username != request.Username)
           {
                return Unauthorized("Invalid username or password.");
           }

            var hasher = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (hasher == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok("Login successful.");
        }
    }
}
