using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PassAuth.Context;
using PassAuth.Models;
using PassAuth.Models.Enums;

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
        public async Task<ActionResult<ManagerRequest>> Post([FromBody] ManagerRequest request)
        { 
            var newRequest = new ManagerRequest
            {
                Title = request.Title,
                Description = request.Description,
                Status = RequestStatus.Pending
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();

            return Created($"/api/requests/{newRequest.Id}", newRequest);
        }
    }
}
