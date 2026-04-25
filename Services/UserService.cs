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

        public async Task DeleteAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user is null)
            {
                throw new KeyNotFoundException("User not found");
            }
            user.isActive = false;
            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await context.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task PromoteAsync(int id, UserRole role)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            

            if (role == 0 )
                throw new InvalidOperationException("Não pode promover para 'User'");
            

            user.Role = role;
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
