using PassAuth.Models.Enums;

namespace PassAuth.DTOs.User
{
    public class ProfileResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty ;
        public UserRole Role { get; set; }
    }
}
