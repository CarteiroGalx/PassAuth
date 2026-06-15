using PassAuth.Models;
namespace PassAuth.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<List<AuditLog>> GetAllAsync();
        Task<AuditLog?> GetAsync(int id);
        Task CreateAsync(AuditLog entity);
        Task CreateAsync(int id, string author, string description);
    }
}
