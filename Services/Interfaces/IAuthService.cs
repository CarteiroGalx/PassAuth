using PassAuth.DTOs.User;

namespace PassAuth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ProfileResponse> Register(RegisterRequest request);
        Task<string> Login(LoginRequest request);
        string GenerateSecurePassword(int length = 12);
    }
}
