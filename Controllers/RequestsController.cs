using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.Request;
using PassAuth.Models;
using PassAuth.Models.Enums;
using PassAuth.Services;
using PassAuth.Services.Interfaces;
using System.Security.Claims;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IAuditLogService _auditService;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public RequestsController(IRequestService requestService, IAuthService authService, IAuditLogService auditService, IUserService userService)
        {
            _requestService = requestService;
            _auditService = auditService;
            _authService = authService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<RequestResponseDto>> Post([FromBody] CreateRequestDto request)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                _authService.ValidateAuthor(authorName!, authorId!, out var verifiedAuthorId);
                var author = await _userService.GetByIdAsync(verifiedAuthorId);
                if (author == null) return Unauthorized();
                _authService.CheckUserStatus(author);
                var auditLog = new AuditLog
                {
                    Author = author.Username,
                    AuthorId = author.Id,
                    Description = author.Username + " criou o request " + request.Title
                };
                await _requestService.CreateAsync(request, author.Id, author.Username);
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

            return Created();
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ManagerRequest>>> GetAll()
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                _authService.ValidateAuthor(authorName!, authorId!, out var verifiedAuthorId);
                var author = await _userService.GetByIdAsync(verifiedAuthorId);
                if (author == null) return Unauthorized();
                _authService.CheckUserStatus(author);
                var auditLog = new AuditLog
                {
                    Author = author.Username,
                    AuthorId = author.Id,
                    Description = author.Username + " buscou por todos os requests dos Managers"
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

            var list = await _requestService.GetAllAsync();
            
            return Ok(list);
        }

        [HttpGet("get-my-requests")]
        public async Task<ActionResult<List<ManagerRequest>>> GetMyRequests()
        {
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (authorId == null) return BadRequest();
            if (!int.TryParse(authorId, out var id)) return Unauthorized();
            await _authService.CheckUserStatusAsync(id);

            var requests = await _requestService.GetByIdAsync(id);

            if(requests.Count == 0) return NoContent();
            return Ok(requests);
        }

        [HttpPatch("{requestId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ManagerRequest>> Validate(int requestId, ValidateRequestDto dto)
        {
            var authorName = User.FindFirst(ClaimTypes.Name)?.Value;
            var authorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            try
            {
                _authService.ValidateAuthor(authorName!, authorId!, out var verifiedAuthorId);
                var author = await _userService.GetByIdAsync(verifiedAuthorId);
                if (author == null) return Unauthorized();
                _authService.CheckUserStatus(author);
                var auditLog = new AuditLog
                {
                    Author = author.Username,
                    AuthorId = author.Id,
                    Description = author.Username + " declarou " + dto.NewStatus.ToString() + " na request " + requestId
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

            var updated = await _requestService.UpdateStatusAsync(requestId, dto.NewStatus);
            return Ok(updated);
        }
    }
}
