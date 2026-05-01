using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Models.Enums;
using PassAuth.Services.Interfaces;

namespace PassAuth.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext context;

        public UserService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<User> AddAsync(CreateUserRequest userDto, string plainPass)
        {
            var novoUsuario = new User
            {
                Username = userDto.Username,
                Role = userDto.Role,
            };
            var hasher = new PasswordHasher<User>();

            novoUsuario.PasswordHash = hasher.HashPassword(novoUsuario, plainPass);

            context.Users.Add(novoUsuario);
            await context.SaveChangesAsync();
            return novoUsuario;
        }

        public async Task ChangeUserStatusAsync(User user, UserStatus newStatus)
        {            
            user.Status = newStatus;
            await context.SaveChangesAsync();
        }

        public async Task ChangeUserStatusAsync(User user, UserStatus newStatus, double suspendExp)
        {
            user.Status = newStatus;
            user.SuspendedUntil = DateTime.UtcNow.AddMinutes(suspendExp);
            await context.SaveChangesAsync();
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            var users = await context.Users.AsNoTracking().ToListAsync();

            return users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                Status = u.Status,
                SuspendedUntil = u.SuspendedUntil
            }).ToList();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        }

        public async Task<UserResponseDto?> GetByIdDtoAsync(int id)
        {
            var user = await context.Users.FindAsync(id);
            var response = new UserResponseDto
            {
                Id = id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Status = user.Status,
                SuspendedUntil = user.SuspendedUntil
            };

            return response;
        }

        public async Task PromoteAsync(int id, NewRoleUserRequest dto)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            

            if (dto.NewRole == 0 )
                throw new InvalidOperationException("Não pode promover para 'User'");
            

            user.Role = dto.NewRole;
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            if(!await UserExistsAsync(user.Id))
                throw new KeyNotFoundException("User not found");

            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user is null)
                return false;
            return true;
        }
    }
}
