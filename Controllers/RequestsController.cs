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

        public RequestsController(IRequestService requestService, IAuthService authService, IAuditLogService auditService)
        {
            _requestService = requestService;
            _auditService = auditService;
            _authService = authService;
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<RequestResponseDto>> Post([FromBody] CreateRequestDto request)
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
                    Description = author.Name + " criou o request " + request.Title
                };
                var createdRequest = await _requestService.CreateAsync(request, author.Id, author.Name);
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
                var author = _authService.ValidateAuthor(authorName!, authorId!);
                var auditLog = new AuditLog
                {
                    Author = author.Name,
                    AuthorId = author.Id,
                    Description = author.Name + " buscou por todos os requests dos Managers"
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

            if (authorId == null)
                return BadRequest();

            if (!int.TryParse(authorId, out var id))
                return Unauthorized();

            var requests = await _requestService.GetByIdAsync(id);

            if(requests.Count == 0) return NoContent();
            return Ok(requests);
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ManagerRequest>> Validate(int requestId, RequestStatus decision)
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
                    Description = author.Name + " declarou " + decision.ToString() + " na request " + requestId
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

            var updated = await _requestService.UpdateStatusAsync(requestId, decision);
            return Ok(updated);
        }
    }
}
