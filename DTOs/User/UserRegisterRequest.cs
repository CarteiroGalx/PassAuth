using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class UserRegisterRequest : UserBaseRequest
    {
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
