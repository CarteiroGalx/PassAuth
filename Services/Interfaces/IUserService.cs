using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Models.Enums;
namespace PassAuth.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<UserResponseDto?> GetByIdDtoAsync(int id);
        Task<bool> UserExistsAsync(int id);
        Task<User> AddAsync(CreateUserRequest user, string plainPass);
        Task UpdateAsync(User user);
        Task PromoteAsync(int id, NewRoleUserRequest dto);
        Task ChangeUserStatusAsync(User user, UserStatus newStatus, double suspendExp);
        Task ChangeUserStatusAsync(User user, UserStatus newStatus);
    }
}
