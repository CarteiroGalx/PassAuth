using PassAuth.DTOs.User;
using PassAuth.Models;

namespace PassAuth.Services.AccountService
{
    public interface IAccountService
    {
        Task<User?> GetByIdAsync(int id);
        Task ChangePasswordAsync(int id, ChangePasswordRequest dto);
    }
}
