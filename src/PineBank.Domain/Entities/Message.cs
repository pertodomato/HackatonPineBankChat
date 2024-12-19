//./src/PineBank.Domain/Entities/Message.cs
namespace PineBank.Domain.Entities
{
    using PineBank.Domain.Common;
    using PineBank.Domain.Exceptions;

    public class Message : EntityBase
    {
        public string Content { get; private set; }
        public string Role { get; private set; } // "user" or "assistant"
        public double Confidence { get; private set; }
        public bool NeedsEmployee { get; private set; }
        public MessageMetadata Metadata { get; private set; }

        private Message() { } // For serialization

        public Message(string content, string role, double confidence = 1.0, bool needsEmployee = false)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainValidationException("Message content cannot be empty");
            Content = content;
            Role = role;
            Confidence = confidence;
            NeedsEmployee = needsEmployee;
            Metadata = new MessageMetadata();
        }

        public void MarkNeedsEmployee(string reason, string specialization)
        {
            NeedsEmployee = true;
            Metadata.EscalationReason = reason;
            Metadata.EmployeeSpecializationNeeded = specialization;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
