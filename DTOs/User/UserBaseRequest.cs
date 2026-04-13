using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class UserBaseRequest
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido")]
        public string Email { get; set; } = string.Empty;
    }
}
