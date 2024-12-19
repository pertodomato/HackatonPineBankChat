//./src/PineBank.Domain/Interfaces/IConversationRepository.cs
namespace PineBank.Domain.Interfaces
{
    using PineBank.Domain.Entities;

    public interface IConversationRepository
    {
        Task<Conversation> GetByIdAsync(Guid id);
        Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId);
        Task<Conversation> AddAsync(Conversation conversation);
        Task UpdateAsync(Conversation conversation);
    }
}
