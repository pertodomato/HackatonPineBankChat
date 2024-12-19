// ./src/PineBank.Domain/Interfaces/IUnitOfWork.cs
using System;
using System.Threading.Tasks;

namespace PineBank.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IConversationRepository Conversations { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
