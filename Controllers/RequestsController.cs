using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.Request;
using PassAuth.Models;
using PassAuth.Models.Enums;
using System.Security.Claims;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager,Admin")]
    public class RequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ManagerRequest>> Post([FromBody] CreateRequestDto request)
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Unauthorized();

            var newRequest = new ManagerRequest
            {
                Title = request.Title,
                Description = request.Description,
                Status = RequestStatus.Pending,
                Author = userName
            };

            var response = new RequestResponseDto
            {
                Title = request.Title,
                Description = request.Description,
                Status = RequestStatus.Pending
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            return Created($"/api/requests/{response.Id}", response);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<ManagerRequest>>> GetAll()
        {
            return await _context.Requests.AsNoTracking().ToListAsync();
        }

        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ManagerRequest>> Validate(int id, RequestStatus decision)
        {
            var targetRequest = await _context.Requests.FindAsync(id);
            targetRequest.Status = decision;
            await _context.SaveChangesAsync();
            return Ok(targetRequest);
        }
    }
}
