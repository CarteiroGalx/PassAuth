using PassAuth.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PassAuth.Models
{
    public class ManagerRequest
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O título é obrigatório")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Título deve estar entre 3 a 20 caracteres")]
        public string Title { get; set; } = string.Empty;
        [Required]
        public RequestStatus Status { get; set; }
        [MaxLength(500, ErrorMessage = "Máximo de caracteres é de 500")]
        public string Description { get; set; } = string.Empty;
    }
}
