using System.ComponentModel.DataAnnotations;

namespace PassAuth.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        [Required]
        public string Author { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
