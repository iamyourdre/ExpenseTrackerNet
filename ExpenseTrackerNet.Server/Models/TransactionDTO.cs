using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNet.Server.Models
{
    public class TransactionWriteDTO
    {
        [Required]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category Id is required")]
        public Guid CategoryId { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

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

        [Required(ErrorMessage = "Category Id is required")]
        public Guid CategoryId { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public int Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class TransactionReadDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
