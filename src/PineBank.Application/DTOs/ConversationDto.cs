//./src/PineBank.Application/DTOs/ConversationDto.cs
namespace PineBank.Application.DTOs
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public bool NeedsEmployeeAttention { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
