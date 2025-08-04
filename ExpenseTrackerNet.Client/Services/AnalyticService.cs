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
    public Dictionary<string, int> Last6MonthsBalance { get; set; } = new();
    public Dictionary<string, int> Last6MonthsIncome { get; set; } = new();
    public Dictionary<string, int> Last6MonthsExpense { get; set; } = new();
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

        var totalIncome = CalculateTotalIncome(transactions);
        var totalExpense = CalculateTotalExpense(transactions);
        var balance = CalculateBalance(transactions);
        var transactionCount = transactions.Count;
        var expenseByCategory = CalculateExpenseByCategory(transactions, totalExpense);
        var last6MonthsBalance = CalculateLast6MonthsMonthlyBalance(transactions);
        var last6MonthsIncome = CalculateLast6MonthsMonthlyIncome(transactions);
        var last6MonthsExpense = CalculateLast6MonthsMonthlyExpense(transactions);

        var result = new AnalyticResultDTO
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            Balance = balance,
            TransactionCount = transactionCount,
            ExpenseByCategory = expenseByCategory,
            Last6MonthsBalance = last6MonthsBalance,
            Last6MonthsIncome = last6MonthsIncome,
            Last6MonthsExpense = last6MonthsExpense
        };
        _js.InvokeVoidAsync("console.log", result);
        return result;
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
        }

        return result;
    }

    private Dictionary<string, int> CalculateLast6MonthsMonthlyBalance(List<TransactionReadDTO> transactions)
    {
        var dict = new Dictionary<string, int>();
        var now = DateTime.Now;
        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var monthKey = month.ToString("yyyy-MM");
            var monthTransactions = transactions.Where(tx => tx.Date.Year == month.Year && tx.Date.Month == month.Month);
            int balance = 0;
            foreach (var tx in monthTransactions)
            {
                if (tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                    balance += tx.Amount;
                else
                    balance -= tx.Amount;
            }
            dict[monthKey] = balance;
        }
        return dict;
    }

    private Dictionary<string, int> CalculateLast6MonthsMonthlyIncome(List<TransactionReadDTO> transactions)
    {
        var dict = new Dictionary<string, int>();
        var now = DateTime.Now;
        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var monthKey = month.ToString("yyyy-MM");
            var monthTransactions = transactions.Where(tx => tx.Date.Year == month.Year && tx.Date.Month == month.Month);
            int income = monthTransactions
                .Where(tx => tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                .Sum(tx => tx.Amount);
            dict[monthKey] = income;
        }
        return dict;
    }

    private Dictionary<string, int> CalculateLast6MonthsMonthlyExpense(List<TransactionReadDTO> transactions)
    {
        var dict = new Dictionary<string, int>();
        var now = DateTime.Now;
        for (int i = 5; i >= 0; i--)
        {
            var month = now.AddMonths(-i);
            var monthKey = month.ToString("yyyy-MM");
            var monthTransactions = transactions.Where(tx => tx.Date.Year == month.Year && tx.Date.Month == month.Month);
            int expense = monthTransactions
                .Where(tx => !tx.Type?.Equals("Income", StringComparison.OrdinalIgnoreCase) == true)
                .Sum(tx => tx.Amount);
            dict[monthKey] = expense;
        }
        return dict;
    }
}
