﻿using ExpenseTrackerNet.Shared.Models;
using ExpenseTrackerNetApp.ApiService.Data;
using ExpenseTrackerNetApp.ApiService.Entities;
using Microsoft.EntityFrameworkCore;

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
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Category = request.Category ?? "Other",
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date
            };
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return new TransactionReadDTO
            {
                Id = transaction.Id,
                Category = transaction.Category,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date
            };
        }

        public async Task<TransactionReadDTO?> UpdateTransactionAsync(Guid userId, TransactionUpdateDTO request)
        {
            var transaction = await _context.Transactions.Where(t => t.Id == request.Id && t.UserId == userId).FirstOrDefaultAsync();
            if (transaction == null)
            {
                return null;
            }
            transaction.Amount = request.Amount;
            transaction.Description = request.Description;
            transaction.Category = request.Category ?? "Other";
            transaction.Date = request.Date;
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return new TransactionReadDTO
            {
                Id = transaction.Id,
                Category = transaction.Category,
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
                    Category = t.Category,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date,
                })
                .FirstOrDefaultAsync();
            if (transaction == null)
                return null;
            return transaction;
        }

        public async Task<IEnumerable<TransactionReadDTO>> GetUserTransactionAsync(Guid userId)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Select(t => new TransactionReadDTO
                {
                    Id = t.Id,
                    Category = t.Category,
                    Amount = t.Amount,
                    Description = t.Description,
                    Date = t.Date
                })
                .ToListAsync();
            if (transactions == null)
                return null;
            return transactions;
        }

        public async Task<bool> DeleteTransactionAsync(Guid userId, Guid id)
        {
            var transaction = await _context.Transactions.Where(t => t.Id == id && t.UserId == userId).FirstOrDefaultAsync();
            if (transaction == null)
            {
                return false;
            }
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
