using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Services.Auth;
namespace PassAuth.Services.User
{
    public class UserService : IUserService
    {
        private readonly AppDbContext context;

        public UserService(AppDbContext context, IAuthService authService)
        {
            this.context = context;
        }

        public async Task<User> AddAsync(CreateUserRequest userDto, string plainPass)
        {
            var novoUsuario = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
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
            context.Users.Remove(user);
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
