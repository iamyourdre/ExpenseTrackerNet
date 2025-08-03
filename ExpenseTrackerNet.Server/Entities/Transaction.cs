using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNetApp.ApiService.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Category { get; set; } = null;
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = "Income";
        public User User { get; set; } = null!;
    }
}