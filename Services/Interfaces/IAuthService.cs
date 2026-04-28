using PassAuth.DTOs.User;
using PassAuth.Models;

namespace PassAuth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ProfileResponse> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        string GenerateSecurePassword(int length = 12);
        Author ValidateAuthor(string id, string name);
        void CheckUserStatus(User user);
        Task CheckUserStatusAsync(int userId);
    }
}
