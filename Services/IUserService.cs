using PassAuth.DTOs;
using PassAuth.Models;
namespace PassAuth.Services
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<User> AddAsync(UserRegisterRequest user, string plainPass);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
