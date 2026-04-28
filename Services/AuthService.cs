using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassAuth.Context;
using PassAuth.DTOs.User;
using PassAuth.Models;
using PassAuth.Models.Enums;
using PassAuth.Services.Interfaces;
using System.Security.Cryptography;


namespace PassAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Login(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                throw new InvalidOperationException("Nome de usuário ou senha inválidos");
            }

            var hasher = new PasswordHasher<User>();
            var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verification == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Nome de usuário ou senha inválidos");
            }

            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role.ToString())
            };

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            if (string.IsNullOrEmpty(key)) throw new InvalidOperationException("JWT key not configured");

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var tokenString = tokenHandler.WriteToken(tokenDescriptor);

            return tokenString;
        }

        public async Task<ProfileResponse> Register(RegisterRequest request)
        {
            var usernameExists = await _context.Users.AnyAsync(u => u.Username == request.Username);
            if (usernameExists)
            {
                throw new InvalidOperationException("Nome de Usuário já existe");
            }

            var newUser = new User
            {
                Username = request.Username,
                Role = UserRole.User
            };

            var hasher = new PasswordHasher<User>();

            newUser.PasswordHash = hasher.HashPassword(newUser, request.Password);

            _context.Users.Add(newUser);

            var response = new ProfileResponse
            {
                Username = request.Username,
                Role = UserRole.User
            };

            await _context.SaveChangesAsync();
            return response;
        }
        public string GenerateSecurePassword(int length = 12)
        {
            char[] characters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@#$%&*".ToCharArray();

            Span<char> result = stackalloc char[length];

            RandomNumberGenerator.GetItems(characters.AsSpan(), result);

            return new string(result);
        }

        public Author ValidateAuthor(string name, string id)
        {
            if (string.IsNullOrEmpty(name)) throw new UnauthorizedAccessException();
            if (!int.TryParse(id, out var authorId) || string.IsNullOrEmpty(id) ) throw new BadHttpRequestException("Token corrompido ou incompleto");
            var author = new Author
            {
                Name = name,
                Id = authorId
            };

            return author;
        }

        public async Task ResetSuspension(User user)
        {
            if (user.SuspendedUntil < DateTime.UtcNow && user.Status == UserStatus.Suspended)
            {
                user.SuspendedUntil = null;
                user.Status = UserStatus.Active;
                await _context.SaveChangesAsync();
            }
        }
    }
}
