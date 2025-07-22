using ExpenseTrackerNet.Shared.Models;

namespace ExpenseTrackerNet.Server.Services
{
    public interface ITransactionService
    {
        Task<TransactionReadDTO?> CreateTransactionAsync(TransactionWriteDTO request);
        Task<TransactionReadDTO?> UpdateTransactionAsync(Guid userId, TransactionUpdateDTO request);
        Task<TransactionReadDTO?> GetTransactionByIdAsync(Guid id);
        Task<IEnumerable<TransactionReadDTO>> GetUserTransactionAsync(Guid userId);
        Task<bool> DeleteTransactionAsync(Guid userId, Guid id);
    }
}
