using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs;
using PassAuth.Models;
using PassAuth.Models.Enums;

namespace PassAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext context;

        public AuthService(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<string> Login(UserDto request)
        {
            throw new NotImplementedException();
        }

        public async Task<UserResponseDto> Register(UserDto request)
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
