using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Models.Enums;

namespace PassAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponse> Login(UserLoginRequest request)
        {
            var user = context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
            if (user.Username != request.Username)
            {
                return Unauthorized("Invalid username or password.");
            }

            var hasher = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);

            if (hasher == PasswordVerificationResult.Failed)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponse> Register(UserRegisterRequest request)
        {
            bool usernameExists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (usernameExists)
            {
                throw new InvalidOperationException("Nome de Usuário já existe");
            }

            bool emailExists = await _context.Users.AnyAsync(e => e.Email == request.Email);
            if (emailExists)
            {
                throw new InvalidOperationException("E-mail já existe");
            }

            var newUser = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = UserRole.User
            };

            var hasher = new PasswordHasher<User>();

            newUser.PasswordHash = hasher.HashPassword(newUser, request.Password);

            _context.Users.Add(newUser);

            var response = new UserResponseDto
            {
                Username = request.Username,
                Email = request.Email
            };

            await _context.SaveChangesAsync();
            return response;
        }
    }
}
