using PassAuth.DTOs.User;

namespace PassAuth.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> Register(UserRegisterDto request);
        Task<string> Login(UserLoginDto request);
    }
}
