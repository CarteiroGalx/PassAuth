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

        public async Task<string> Login(UserLoginDto request)
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

        public async Task<UserResponseDto> Register(UserRegisterDto request)
        {
            var newUser = new User
            {
                Username = request.Username,
                Role = UserRole.User
            };

            var hasher = new PasswordHasher<User>();

            newUser.PasswordHash = hasher.HashPassword(newUser, request.Password);

            context.Users.Add(newUser);

            var response = new UserResponseDto
            {
                Username = request.Username,
            };

            await context.SaveChangesAsync();
            return response;
        }
    }
}
