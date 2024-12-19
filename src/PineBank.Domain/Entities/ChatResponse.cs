// ./src/PineBank.Domain/Entities/ChatResponse.cs

namespace PineBank.Domain.Entities
{
    public class ChatResponse
    {
        public string ResponseMessage { get; set; }
        public ChatResponseMetadata Metadata { get; set; }
        // Outras propriedades conforme necessário
    }

    public class ChatResponseMetadata
    {
        public string Topic { get; set; }
        public string Intent { get; set; }
        public string EscalationReason { get; set; }
        public string EmployeeSpecializationNeeded { get; set; }
        // Outras propriedades conforme necessário
    }
}
