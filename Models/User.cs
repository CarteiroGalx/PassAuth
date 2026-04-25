using Microsoft.EntityFrameworkCore;
using PassAuth.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PassAuth.Models
{
    [Index(nameof(Username), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class User
    {
        [Key] 
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O usuário deve ter entre 3 e 20 caracteres.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}