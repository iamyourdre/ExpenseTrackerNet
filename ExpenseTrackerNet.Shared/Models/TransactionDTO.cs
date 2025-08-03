using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNet.Shared.Models
{
    public class TransactionWriteDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters")]
        public string Description { get; set; } = string.Empty;

        public string? Category { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "Income";

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class TransactionUpdateDTO
    {
        [Required(ErrorMessage = "Transaction's id is required")]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters")]
        public string Description { get; set; } = string.Empty;

        public string? Category { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; } = "Income";

        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class TransactionReadDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Category { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string? Type { get; set; }
        public DateTime Date { get; set; }
    }
}
