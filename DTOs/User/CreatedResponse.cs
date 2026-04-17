using PassAuth.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class CreatedResponse : BaseRequest
    {
        public int Id { get; set; }
        public UserRole Role { get; set; }
        public string Password {  get; set; } = string.Empty;
    }
}
