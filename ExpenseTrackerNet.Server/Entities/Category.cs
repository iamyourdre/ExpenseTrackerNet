using ExpenseTrackerNetApp.ApiService.Entities;

namespace ExpenseTrackerNet.Server.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public User User { get; set; } = null!; // Navigation property to User entity
    }
}
