using PassAuth.DTOs;

namespace PassAuth.Services
{
    public interface IAuthService
    {
        Task Register(UserDto request);
        Task<bool> Login(UserDto request);
    }
}
