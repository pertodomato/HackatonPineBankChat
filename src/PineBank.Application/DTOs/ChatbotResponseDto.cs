//./src/PineBank.Application/DTOs/ChatbotResponseDto.cs
using PineBank.Application.DTOs;

namespace PineBank.Application.DTOs
{
    public class ChatbotResponseDto
    {
        public string Message { get; set; }
        public double Confidence { get; set; }
        public bool NeedsEmployee { get; set; }
        public string EscalationReason { get; set; }
        public List<string> SuggestedActions { get; set; }
        public ChatbotResponseMetadataDto Metadata { get; set; } = new();
    }

    public class ChatbotResponseMetadataDto
    {
        public string Topic { get; set; }
        public string Intent { get; set; }
        public string EscalationReason { get; set; }
        public string EmployeeSpecializationNeeded { get; set; }
    }
}
