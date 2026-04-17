using PassAuth.DTOs.User;

namespace PassAuth.Services
{
    public interface IAuthService
    {
        Task<Response> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        string GenerateSecurePassword(int length = 12);
    }
}
