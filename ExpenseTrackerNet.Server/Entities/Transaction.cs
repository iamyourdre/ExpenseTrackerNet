using ExpenseTrackerNet.Server.Entities;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNetApp.ApiService.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // Foreign key to User entity
        public Guid CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public User User { get; set; } = null!; // Navigation property to User entity
        public Category Category { get; set; } = null!; // Navigation property to Category entity
    }
}