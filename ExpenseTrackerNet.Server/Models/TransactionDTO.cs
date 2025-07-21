using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNet.Server.Models
{
    public class TransactionWriteDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }

    public class TransactionReadDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public DateTime Date { get; set; }

        // Optional: extra info for UI (e.g., username)
        public string Username { get; set; } = string.Empty;
    }
}
