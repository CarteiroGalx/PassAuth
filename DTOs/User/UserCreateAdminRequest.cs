using PassAuth.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class UserCreateAdminRequest : UserBaseRequest
    {
        [Required(ErrorMessage = "O cargo é obrigatório")]
        public UserRole Role { get; set; }
    }
}
