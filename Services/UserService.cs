using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.User;
using PassAuth.Models;
namespace PassAuth.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext context;

        public UserService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<User> AddAsync(UserRegisterRequest userDto, string plainPass)
        {
            var novoUsuario = new User
            {
                Username = userDto.Username
            };

            var passwordHasher = new PasswordHasher<User>();

            novoUsuario.PasswordHash = passwordHasher.HashPassword(novoUsuario, userDto.Password);

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
