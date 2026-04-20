using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.User;
using PassAuth.Models;

namespace PassAuth.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task ChangePasswordAsync(int id, ChangePasswordRequest dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            var hasher = new PasswordHasher<User>();
            var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.CurrentPassword);

            if (verification == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Senha atual inválida");
            }

            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
            await _context.SaveChangesAsync();
        }
    }
}
