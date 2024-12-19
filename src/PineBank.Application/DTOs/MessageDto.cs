//./src/PineBank.Application/DTOs/MessageDto.cs
namespace PineBank.Application.DTOs
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Role { get; set; }
        public double Confidence { get; set; }
        public bool NeedsEmployee { get; set; }
        public MessageMetadataDto Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
