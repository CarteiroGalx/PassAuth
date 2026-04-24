using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.Request;
using PassAuth.Models;
using PassAuth.Models.Enums;
using PassAuth.Services.Interfaces;
using System.Security.Claims;

namespace PassAuth.Services
{
    public class RequestService : IRequestService
    {
        private readonly AppDbContext _context;

        public RequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ManagerRequest> CreateAsync(CreateRequestDto dto, int authorId, string authorName)
        {
            var newRequest = new ManagerRequest
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = RequestStatus.Pending,
                Author = authorName,
                AuthorId = authorId,
                PublishedAt = DateTime.UtcNow,
            };

            _context.Requests.Add(newRequest);
            await _context.SaveChangesAsync();
            return newRequest;
        }

        public async Task<List<ManagerRequest>> GetAllAsync()
        {
            return await _context.Requests.AsNoTracking().ToListAsync();
        }

        public async Task<List<ManagerRequest>> GetByIdAsync(int id)
        {
            return await _context.Requests.Where(u => u.AuthorId == id).ToListAsync();
        }

        public async Task<ManagerRequest> UpdateStatusAsync(int id, RequestStatus decision)
        {
            var target = await _context.Requests.FindAsync(id);
            if (target == null) throw new KeyNotFoundException("Request not found");
            target.Status = decision;
            target.ReviewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return target;
        }
    }
}
