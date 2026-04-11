using Microsoft.AspNetCore.Mvc;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Services;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService service;

        public UsersController(IUserService service)
        {
            this.service = service;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await service.GetAllAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await service.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            try
            {
                await service.UpdateAsync(user);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserRegisterRequest user)
        {
            var newUser = await service.AddAsync(user, user.Password);

            var userResponse = new
            {
                id = newUser.Id,
                userName = newUser.Username
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, userResponse);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { e = ex.Message });
            }
        }
    }
}
