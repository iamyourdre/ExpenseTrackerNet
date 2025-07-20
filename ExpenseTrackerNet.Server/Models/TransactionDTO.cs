namespace ExpenseTrackerNet.Server.Models
{
    public class TransactionDTO
    {
        public Guid UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Amount { get; set; }
    }
}
