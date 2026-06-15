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
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _requestService.CreateAsync(request, author.Id, author.Username);
                await _auditService.CreateAsync(author.Id, author.Username, $"{author.Username} criou a request: {request.Title}");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message });
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
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, $"{author.Username} buscou por todos os Requests");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (BadHttpRequestException)
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
                var author = await _authService.ValidateUserAsync(authorName!, authorId!);
                await _auditService.CreateAsync(author.Id, author.Username, $"{author.Username} declarou {dto.NewStatus} na request {requestId}");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch (BadHttpRequestException)
            {
                return BadRequest();
            }

            var updated = await _requestService.UpdateStatusAsync(requestId, dto.NewStatus);
            return Ok(updated);
        }
    }
}
