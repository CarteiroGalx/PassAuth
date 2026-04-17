using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "A nova senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter pelo menos 6 caractéres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Digite a senha de confirmação")]
        [Compare("CurrentPassword", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmationPassword { get; set; } = string.Empty;
    }
}
