using PassAuth.DTOs.User;
using PassAuth.Models;

namespace PassAuth.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> GetByIdAsync(int id);
        Task ChangePasswordAsync(int id, ChangePasswordRequest dto);
    }
}
