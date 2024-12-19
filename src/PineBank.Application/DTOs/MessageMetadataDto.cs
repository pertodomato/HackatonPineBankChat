//./src/PineBank.Application/DTOs/MessageMetadataDto.cs

namespace PineBank.Application.DTOs
{
    
    public class MessageMetadataDto
    {
        public string Intent { get; set; }
        public string Topic { get; set; }
        public string EscalationReason { get; set; }
        public string EmployeeSpecializationNeeded { get; set; }
        public Dictionary<string, string> AdditionalData { get; set; }
    }
}
