using PassAuth.Context;
using PassAuth.DTOs;

namespace PassAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext context;

        public AuthService(AppDbContext context)
        {
            this.context = context;
        }

        public Task<string> Login(UserDto request)
        {
            throw new NotImplementedException();
        }

        public Task Register(UserDto request)
        {
            throw new NotImplementedException();
        }
    }
}
