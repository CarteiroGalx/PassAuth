using PassAuth.DTOs;

namespace PassAuth.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> Register(UserDto request);
        Task<string> Login(UserDto request);
    }
}
