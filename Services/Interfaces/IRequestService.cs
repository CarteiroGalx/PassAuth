using PassAuth.DTOs.Request;
using PassAuth.Models;
using PassAuth.Models.Enums;

namespace PassAuth.Services.Interfaces
{
    public interface IRequestService
    {
        Task<ManagerRequest> CreateAsync(CreateRequestDto dto, int authorId, string authorName);
        Task<List<ManagerRequest>> GetAllAsync();
        Task<List<ManagerRequest>> GetByIdAsync(int id);
        Task<ManagerRequest> UpdateStatusAsync(int id, RequestStatus decision);
    }
}
