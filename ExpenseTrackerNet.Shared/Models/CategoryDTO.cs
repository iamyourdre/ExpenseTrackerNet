using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerNet.Shared.Models
{
    public class CategoryWriteDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Icon { get; set; }
    }

    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "Category's id is required")]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        public string Icon { get; set; }
    }

    public class CategoryReadDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
