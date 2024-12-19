// ./src/PineBank.API/Models/Responses/ChatResponse.cs
using System.Collections.Generic;

namespace PineBank.API.Models.Responses
{
    public class ChatResponse
    {
        public string Message { get; set; }
        public double Confidence { get; set; }
        public bool NeedsEmployee { get; set; }
        public string EscalationReason { get; set; }
        public List<string> SuggestedActions { get; set; }
        public ChatResponseMetadata Metadata { get; set; }
    }

    public class ChatResponseMetadata
    {
        public string Topic { get; set; }
        public string Intent { get; set; }
        public string EmployeeSpecializationNeeded { get; set; }
    }
}
