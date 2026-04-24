using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.Models;
using PassAuth.Services.Interfaces;

namespace PassAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _service;

        public AuditLogsController(IAuditLogService service)
        {
            _service = service;
        }

        [HttpGet("audit")]
        public async Task<ActionResult<List<AuditLog>>> GetAuditLogs()
        {
            var events = await _service.GetAllAsync();
            return Ok(events);
        }

        [HttpGet]
        public async Task<ActionResult<AuditLog>> GetById(int id)
        {
            var auditLog = await _service.GetAsync(id);
            if (auditLog == null) return NotFound();
            return Ok(auditLog);
        }

    }
}
