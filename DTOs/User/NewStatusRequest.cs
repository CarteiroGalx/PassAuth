using PassAuth.Models.Enums;

namespace PassAuth.DTOs.User
{
    public class NewStatusRequest
    {
        public UserStatus NewStatus { get; set; }
        public string Reason { get; set; } = string.Empty;
        public double? SuspendedExp { get; set; }
    }
}
