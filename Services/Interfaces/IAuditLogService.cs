using PassAuth.Models;
namespace PassAuth.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<List<AuditLog>> GetAllAsync();
        Task<AuditLog?> GetAsync(int id);
        Task CreateAsync(AuditLog entity, int authorId);
    }
}
