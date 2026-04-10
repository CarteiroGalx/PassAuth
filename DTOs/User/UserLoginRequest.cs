using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}
