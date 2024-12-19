//./src/PineBank.Domain/Entities/MessageMetadata.cs
namespace PineBank.Domain.Entities
{
    public class MessageMetadata
    {
        public string Intent { get; set; }
        public string Topic { get; set; }
        public string EscalationReason { get; set; }
        public string EmployeeSpecializationNeeded { get; set; }
        public Dictionary<string, string> AdditionalData { get; private set; } = new();
    }
}
