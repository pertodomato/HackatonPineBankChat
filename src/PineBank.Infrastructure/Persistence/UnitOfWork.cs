// ./src/PineBank.Infrastructure/Persistence/UnitOfWork.cs
using PineBank.Domain.Interfaces;
using MongoDB.Driver;

namespace PineBank.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MongoDbContext _context;
        private IConversationRepository _conversations;
        private IClientSessionHandle _session;

        public UnitOfWork(MongoDbContext context)
        {
            _context = context;
        }

        public IConversationRepository Conversations => _conversations ??= new ConversationRepository(_context);

        public async Task BeginTransactionAsync()
        {
            _session = await _context.Database.Client.StartSessionAsync();
            _session.StartTransaction();
        }

        public async Task CommitAsync()
        {
            await _session.CommitTransactionAsync();
            _session.Dispose();
        }

        public async Task RollbackAsync()
        {
            await _session.AbortTransactionAsync();
            _session.Dispose();
        }

        public async Task<bool> SaveChangesAsync()
        {
            // MongoDB operations are immediate; return true
            return true;
        }

        public void Dispose()
        {
            _session?.Dispose();
        }
    }
}
