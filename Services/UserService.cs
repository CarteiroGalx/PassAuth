using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs;
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

        public async Task<User> AddAsync(UserDto userDto, string plainPass)
        {
            // 1. Transformamos o que veio da web (DTO) em um objeto de banco (Entity)
            var novoUsuario = new User
            {
                Username = userDto.Username
                // Não preenchemos a senha ainda!
            };

            // 2. Instanciamos o Hasher com o tipo da ENTITY
            var passwordHasher = new PasswordHasher<User>();

            // 3. Geramos o hash usando o próprio objeto que será salvo
            novoUsuario.PasswordHash = passwordHasher.HashPassword(novoUsuario, userDto.Password);

            // 4. Salvamos
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
