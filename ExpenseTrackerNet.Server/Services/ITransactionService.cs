using ExpenseTrackerNet.Server.Models;

namespace ExpenseTrackerNet.Server.Services
{
    public interface ITransactionService
    {
        Task<TransactionReadDTO?> CreateTransactionAsync(TransactionWriteDTO request);
        Task<TransactionReadDTO?> GetTransactionByIdAsync(Guid id);
        Task<IEnumerable<TransactionReadDTO>> GetUserTransactionAsync(Guid userId);
    }
}
