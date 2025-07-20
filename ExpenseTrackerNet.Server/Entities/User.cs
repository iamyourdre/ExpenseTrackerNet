using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNetApp.ApiService.Entities
{
    public class User
    {
        public Guid Id { get; set; }


        [Required]
        [MinLength(3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<Transaction> Transactions { get; set; } = [];
    }
}
