using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.Models;
using PassAuth.Services.Interfaces;

namespace PassAuth.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;

        public AuditLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AuditLog entity)
        {
            entity.OccurredAt = DateTime.UtcNow;
            _context.Audit.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _context.Audit.AsNoTracking().OrderByDescending(e => e.OccurredAt).ToListAsync();
        }

        public async Task<AuditLog?> GetAsync(int id)
        {
            return await _context.Audit.FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
