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
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _authService.ValidateAuthor(authorName!, authorId!, out var id);
            var author = await _userService.GetByIdAsync(id);
            if (author == null) return Unauthorized();

            try
            {
                _authService.CheckUserStatus(author);
                var auditLog = new AuditLog
                {
                    Author = author.Username,
                    AuthorId = author.Id,
                    Description = author.Username + " buscou por todos os usuários"
                };

                await _auditService.CreateAsync(auditLog);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message});
            }

            return await _userService.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = _authService.ValidateAuthor(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Name,
                    AuthorId = author.Id,
                    Description = author.Name + " buscou pelo usuário de ID: " + id
                };

                await _auditService.CreateAsync(auditLog);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

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
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = _authService.ValidateAuthor(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Name,
                    AuthorId = author.Id,
                    Description = author.Name + " modificou dados do usuário de ID: " + id
                };

                await _auditService.CreateAsync(auditLog);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }

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

        [HttpPatch("promote-user/{id}")]
        public async Task<ActionResult> PromoteUser(int id, UserRole newRole)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var author = _authService.ValidateAuthor(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Name,
                    AuthorId = author.Id,
                    Description = author.Name + " promoveu usuário de ID: " + id + " para " + newRole
                };

                await _auditService.CreateAsync(auditLog);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }


            try
            {
                await _userService.PromoteAsync(id, newRole);
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
                var userAuthor = await _userService.GetByIdAsync(id);
                if(userAuthor is null) return Unauthorized();
                await _authService.ValidateAcess(userAuthor);
                var author = _authService.ValidateAuthor(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Name,
                    AuthorId = author.Id,
                    Description = author.Name + " criou o usuário " + user.Username
                };

                await _auditService.CreateAsync(auditLog);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
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
        public async Task<IActionResult> ChangerUserStatus(int id, UserStatus newStatus, string reason, double? suspendedExp)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user is null) return NotFound();
                var author = _authService.ValidateAuthor(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Name,
                    AuthorId = author.Id,
                    Description = author.Name + " alterou os status de usuário '" + id + "' para " + newStatus + ". " +
                    "Justificativa: " + reason
                };

                if (suspendedExp > 0 && newStatus == UserStatus.Suspended)
                {
                    var minutes = suspendedExp.Value;
                    await _userService.ChangeUserStatusAsync(user, newStatus, minutes);
                    auditLog.Description += ". Tempo de suspensão: " + minutes + " minutos";
                }
                else if (newStatus == UserStatus.Suspended)
                    return BadRequest( new {message = "Tempo de suspensão é obrigatório"});
                else
                    await _userService.ChangeUserStatusAsync(user, newStatus);

                await _auditService.CreateAsync(auditLog);
                return Ok(new {message = "Usuário " + user.Id + " está marcado agora como " + user.Status});

            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
