using PassAuth.DTOs.User;

namespace PassAuth.Services
{
    public interface IAuthService
    {
        Task<UserResponse> Register(UserRegisterRequest request);
        Task<UserResponse> Login(UserLoginRequest request);
    }
}
