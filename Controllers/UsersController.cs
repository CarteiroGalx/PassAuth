using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Services.Auth;
using PassAuth.Services.User;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            try
            {
                await _userService.UpdateAsync(user);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(CreateUserRequest user)
        {
            var temporaryPass = _authService.GenerateSecurePassword();
            var newUser = await _userService.AddAsync(user, temporaryPass);

            var userResponse = new
            {
                id = newUser.Id,
                userName = newUser.Username,
                email = newUser.Email,
                role = newUser.Role,
                password = temporaryPass,
                message = "IMPORTANTE: Em produção, esta senha seria enviada por e-mail. " +
                            "Exibida aqui apenas para facilitar testes da API."
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, userResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { e = ex.Message });
            }
        }
    }
}
