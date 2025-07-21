using ExpenseTrackerNet.Server.Models;
using ExpenseTrackerNetApp.ApiService.Data;
using ExpenseTrackerNetApp.ApiService.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace ExpenseTrackerNet.Server.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly IConfiguration _configuration;

        public TransactionService(ExpenseTrackerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TransactionReadDTO?> CreateTransactionAsync(TransactionWriteDTO request)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);
            bool isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);

            if (!isValid)
            {
                // Remove the usage of ModelState and replace it with validationResults
                var errors = validationResults
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return null;
            }
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return new TransactionReadDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date
            };
        }

        public async Task<TransactionReadDTO?> UpdateTransactionAsync(TransactionUpdateDTO request)
        {
            var transaction = await _context.Transactions.FindAsync(request.Id);
            if (transaction == null)
            {
                return null;
            }
            transaction.Amount = request.Amount;
            transaction.Description = request.Description;
            transaction.Date = request.Date;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return new TransactionReadDTO
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date
            };
        }

        public async Task<TransactionReadDTO?> GetTransactionByIdAsync(Guid id)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id)
                .Select(t => new TransactionReadDTO
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date
                })
                .FirstOrDefaultAsync();
            return transaction;
        }

        public async Task<IEnumerable<TransactionReadDTO>> GetUserTransactionAsync(Guid userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new TransactionReadDTO
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date
                })
                .ToListAsync();
            return transactions;
        }
    }
}
