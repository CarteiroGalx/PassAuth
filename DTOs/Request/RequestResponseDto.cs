using PassAuth.Models.Enums;

namespace PassAuth.DTOs.Request
{
    public class RequestResponseDto
    {
        public RequestStatus Status { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
