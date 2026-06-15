using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Models.Enums;
using PassAuth.Services;
using PassAuth.Services.Interfaces;
using System.Security.Claims;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IAuditLogService _auditService;

        public UsersController(IUserService userService, IAuthService authService, IAuditLogService auditService)
        {
            _userService = userService;
            _authService = authService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, author.Username + " buscou por todos os usuários");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return await _userService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, author.Username + " buscou pelo usuário de ID: " + id);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var user = await _userService.GetByIdDtoAsync(id);
            if (user == null) return NotFound();

            return user;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, ModifyUserRequest dto)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, author.Username + " editou o usuário de ID: " + id);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var user = await _userService.GetByIdAsync(id);
            if(user == null) return NotFound();

            user.Username = dto.Username;
            user.Role = dto.Role;
            user.Email = dto.Email;

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

        [HttpPatch("change-role/{id}")]
        public async Task<ActionResult> ChangerUserRole(int id, NewRoleUserRequest dto)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, author.Username + " promoveu usuário de ID: " + id + " para " + dto.NewRole);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            try
            {
                await _userService.PromoteAsync(id, dto);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(CreateUserRequest user)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(!int.TryParse(authorId, out var id)) return BadRequest();

            try
            {
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, author.Username + " criou o usuário " + user.Username);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            var temporaryPass = _authService.GenerateSecurePassword();
            var newUser = await _userService.AddAsync(user, temporaryPass);

            var userResponse = new
            {
                id = newUser.Id,
                userName = newUser.Username,
                role = newUser.Role,
                password = temporaryPass,
                message = "IMPORTANTE: Em produção, esta senha seria enviada por e-mail. " +
                            "Exibida aqui apenas para facilitar testes da API."
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, userResponse);
        }

        [HttpPatch("change-status/{id}")]
        public async Task<IActionResult> ChangerUserStatus(int id, NewStatusRequest dto)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Username,
                    AuthorId = author.Id,
                    Description = author.Username + " declarou " + dto.NewStatus.ToString() + " para o usuário de ID: " + id
                };

                var user = await _userService.GetByIdAsync(id);
                if(user == null) return NotFound();
                if (dto.NewStatus == UserStatus.Suspended)
                {
                    if (dto.SuspendedExp > 0)
                    {
                        var minutes = dto.SuspendedExp.Value;
                        await _userService.ChangeUserStatusAsync(user, dto.NewStatus, minutes);
                        auditLog.Description += ". Tempo de suspensão: " + minutes + " minutos";
                    }
                    if(dto.SuspendedExp <= 0)
                    {
                        return BadRequest(new { message = "Tempo de suspensão é obrigatório" });
                    }
                }
                else
                    await _userService.ChangeUserStatusAsync(user, dto.NewStatus);

                await _auditService.CreateAsync(auditLog);
                return Ok(new {message = "Usuário " + user.Id + " está marcado agora como " + user.Status});

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
