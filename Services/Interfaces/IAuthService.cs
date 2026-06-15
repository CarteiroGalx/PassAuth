using PassAuth.DTOs.User;
using PassAuth.Models;

namespace PassAuth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ProfileResponse> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        string GenerateSecurePassword(int length = 12);
        Task<User> ValidateUser(string name, string id);
        Task CheckUserStatusAsync(int userId);
    }
}
