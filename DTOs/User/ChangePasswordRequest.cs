using System.ComponentModel.DataAnnotations;

namespace PassAuth.DTOs.User
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "A senha atual é obrigatória")]
        public string ActualPass { get; set; }
        [Required(ErrorMessage = "A nova senha é obrigatória")]
        public string NewPass { get; set; }
        [Required(ErrorMessage = "Digite a senha de confirmação")]
        [Compare("ActualPass", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmationPass { get; set; }
    }
}
