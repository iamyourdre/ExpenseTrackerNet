using ExpenseTrackerNet.Server.Models;

namespace ExpenseTrackerNet.Server.Services
{
    public interface ICategoryService
    {
        Task<CategoryReadDTO?> CreateCategoryAsync(CategoryWriteDTO request);
        Task<CategoryReadDTO?> UpdateCategoryAsync(Guid userId, CategoryUpdateDTO request);
        Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id);
        Task<IEnumerable<CategoryReadDTO>> GetUserCategoriesAsync(Guid userId);
        Task<bool> DeleteCategoryAsync(Guid userId, Guid id);
    }
}
