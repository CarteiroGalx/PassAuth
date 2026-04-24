using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.Request;
using PassAuth.Models;
using PassAuth.Models.Enums;
using PassAuth.Services.Interfaces;
using System.Security.Claims;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _service;

        public RequestsController(IRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<RequestResponseDto>> Post([FromBody] CreateRequestDto request)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            if (!int.TryParse(userId, out var id))
                return BadRequest();

            var created = await _service.CreateAsync(request, id, userName);

            var response = new RequestResponseDto
            {
                Id = id,
                Title = created.Title,
                Description = created.Description,
                Status = created.Status
            };

            return Created();
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ManagerRequest>>> GetAll()
        {
            var list = await _service.GetAllAsync();
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

            var requests = await _service.GetByIdAsync(id);

            if(requests.Count == 0) return NoContent();
            return Ok(requests);
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ManagerRequest>> Validate(int id, RequestStatus decision)
        {
            var updated = await _service.UpdateStatusAsync(id, decision);
            return Ok(updated);
        }
    }
}
