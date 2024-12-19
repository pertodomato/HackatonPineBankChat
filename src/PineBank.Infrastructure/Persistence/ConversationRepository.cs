// ./src/PineBank.Infrastructure/Persistence/ConversationRepository.cs
using MongoDB.Driver;
using PineBank.Domain.Entities;
using PineBank.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace PineBank.Infrastructure.Persistence
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly IMongoCollection<Conversation> _conversations;
        private readonly ILogger<ConversationRepository> _logger;

        public ConversationRepository(MongoDbContext context)
        {
            _conversations = context.Conversations;
        }

        public async Task<Conversation> AddAsync(Conversation conversation)
        {
            await _conversations.InsertOneAsync(conversation);
            return conversation;
        }

        public async Task<Conversation> GetByIdAsync(Guid id)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, id);
            return await _conversations.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Conversation>> GetByUserIdAsync(string userId)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.UserId, userId);
            return await _conversations.Find(filter).ToListAsync();
        }

        public async Task UpdateAsync(Conversation conversation)
        {
            var filter = Builders<Conversation>.Filter.Eq(c => c.Id, conversation.Id);
            await _conversations.ReplaceOneAsync(filter, conversation);
        }
    }
}