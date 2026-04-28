using Microsoft.EntityFrameworkCore;
using PassAuth.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PassAuth.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        [Key] 
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "O usuário deve ter entre 3 e 20 caracteres.")]
        public string Username { get; set; } = string.Empty;
        [Required(ErrorMessage = "E-mail é obrigatório")]
        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;
        [DataType(DataType.Date)]
        public DateTime? SuspendedUntil { get; set; }
    }
}