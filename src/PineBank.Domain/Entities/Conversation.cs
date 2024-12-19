//./src/PineBank.Domain/Entities/Conversation.cs
namespace PineBank.Domain.Entities
{
    using PineBank.Domain.Common;
    using PineBank.Domain.Exceptions;

    public class Conversation : EntityBase
    {
        private readonly List<Message> _messages = new();
        public string UserId { get; private set; }
        public bool IsActive { get; private set; }
        public bool NeedsEmployeeAttention { get; private set; }
        public IReadOnlyCollection<Message> Messages => _messages.AsReadOnly();

        private Conversation() { } // For serialization

        public Conversation(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new DomainValidationException("UserId cannot be empty");
            UserId = userId;
            IsActive = true;
        }

        public void AddMessage(Message message)
        {
            if (!IsActive)
                throw new DomainValidationException("Cannot add message to inactive conversation");
            _messages.Add(message);
            UpdatedAt = DateTime.UtcNow;
            if (message.NeedsEmployee)
                NeedsEmployeeAttention = true;
        }

        public void Close()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
