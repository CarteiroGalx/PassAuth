using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class BaseRequestUser
    {
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;
    }
}
