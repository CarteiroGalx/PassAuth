using PassAuth.DTOs.User;
using PassAuth.Models;

namespace PassAuth.Services.Interfaces
{
    public interface IAccountService
    {
        Task<User?> GetByIdAsync(int id);
        Task ChangePasswordAsync(User user, ChangePasswordRequest dto);
        Task ResetSuspensionAsync(User user);
    }
}
