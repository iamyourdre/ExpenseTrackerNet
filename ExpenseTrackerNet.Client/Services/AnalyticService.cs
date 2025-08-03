using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ExpenseTrackerNet.Shared.Models;

namespace ExpenseTrackerNet.Client.Services;

public interface IAnalyticService
{
    Task<AnalyticResultDTO?> GetAnalyticsAsync();
}

public class AnalyticResultDTO
{
    public int TotalIncome { get; set; }
    public int TotalExpense { get; set; }
    public int Balance { get; set; }
    public int TransactionCount { get; set; }
    public Dictionary<string, CategoryExpenseDTO> ExpenseByCategory { get; set; } = new();
    public int Last7DaysBalance { get; set; }
    public int Last7DaysIncomeExpenseDiff { get; set; }
}

public class CategoryExpenseDTO
{
    public int Amount { get; set; }
    public double Percentage { get; set; }
}

public class AnalyticService : BaseService, IAnalyticService
{
    private readonly ITransactionService _transactionService;

    public AnalyticService(
        IHttpClientFactory factory,
        IJSRuntime js,
        NavigationManager nav,
        ITransactionService transactionService)
        : base(factory, js, nav)
    {
        _transactionService = transactionService;
    }

    public async Task<AnalyticResultDTO?> GetAnalyticsAsync()
    {
        var transactions = await _transactionService.GetUserTransactionsAsync();
        if (transactions == null || transactions.Count == 0)
            return null;

        //await _js.InvokeVoidAsync("console.log", transactions.Select(t => new { t.Type, t.Category, t.Amount }));

        var totalIncome = CalculateTotalIncome(transactions);
        var totalExpense = CalculateTotalExpense(transactions);
        var balance = CalculateBalance(transactions);
        var transactionCount = transactions.Count;
        var expenseByCategory = CalculateExpenseByCategory(transactions, totalExpense);
        var last7DaysBalance = CalculateLast7DaysBalance(transactions);
        var last7DaysIncomeExpenseDiff = CalculateLast7DaysIncomeExpenseDiff(transactions);

        return new AnalyticResultDTO
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = balance,
            TransactionCount = transactionCount,
            ExpenseByCategory = expenseByCategory,
            Last7DaysBalance = last7DaysBalance,
            Last7DaysIncomeExpenseDiff = last7DaysIncomeExpenseDiff
        };

    }

    private int CalculateTotalIncome(List<TransactionReadDTO> transactions) =>
        transactions.Where(tx => tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                    .Sum(tx => tx.Amount);

    private int CalculateTotalExpense(List<TransactionReadDTO> transactions) =>
        transactions.Where(tx => !tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                    .Sum(tx => tx.Amount);

    private int CalculateBalance(List<TransactionReadDTO> transactions)
    {
        int balance = 0;
        foreach (var tx in transactions)
        {
            if (tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                balance += tx.Amount;
            else
                balance -= tx.Amount;
        }
        return balance;
    }

    private Dictionary<string, CategoryExpenseDTO> CalculateExpenseByCategory(
        List<TransactionReadDTO> transactions, int totalExpense)
    {
        var expenseByCategory = new Dictionary<string, int>();

        foreach (var tx in transactions)
        {
            if (tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                continue;

            var category = tx.Category ?? "Other";
            if (!expenseByCategory.ContainsKey(category))
                expenseByCategory[category] = 0;
            expenseByCategory[category] += tx.Amount;
            Console.WriteLine($"Category: {category}, Amount: {tx.Amount}");
        }

        var result = new Dictionary<string, CategoryExpenseDTO>();
        foreach (var kvp in expenseByCategory)
        {
            double percent = totalExpense > 0 ? (kvp.Value * 100.0) / totalExpense : 0;
            result[kvp.Key] = new CategoryExpenseDTO
            {
                Amount = kvp.Value,
                Percentage = Math.Round(percent, 2)
            };
            Console.WriteLine($"Category: {kvp.Key}, Amount: {kvp.Value}, Percentage: {percent}%");
        }

        return result;
    }

    private int CalculateLast7DaysBalance(List<TransactionReadDTO> transactions)
    {
        var last7Days = DateTime.Now.AddDays(-6).Date;
        int balance = 0;
        foreach (var tx in transactions.Where(tx => tx.Date.Date >= last7Days))
        {
            if (tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                balance += tx.Amount;
            else
                balance -= tx.Amount;
        }
        return balance;
    }

    private int CalculateLast7DaysIncomeExpenseDiff(List<TransactionReadDTO> transactions)
    {
        var last7Days = DateTime.Now.AddDays(-6).Date;
        int income = 0, expense = 0;
        foreach (var tx in transactions.Where(tx => tx.Date.Date >= last7Days))
        {
            if (tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                income += tx.Amount;
            else
                expense += tx.Amount;
        }
        return income - expense;
    }
}
