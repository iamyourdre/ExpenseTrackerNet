using ExpenseTrackerNet.Server.Entities;
using ExpenseTrackerNet.Shared.Models;
using ExpenseTrackerNetApp.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTrackerNet.Server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ExpenseTrackerDbContext _context;
        private readonly IConfiguration _configuration;

        public CategoryService(ExpenseTrackerDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<CategoryReadDTO?> CreateCategoryAsync(CategoryWriteDTO request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Category data is required.");
            }
            var category = new Category
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Name = request.Name,
                Icon = request.Icon ?? string.Empty
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon
            };
        }

        public async Task<CategoryReadDTO?> UpdateCategoryAsync(Guid userId, CategoryUpdateDTO request)
        {
            var category = await _context.Categories
                .Where(c => c.Id == request.Id && c.UserId == userId)
                .FirstOrDefaultAsync();
            if(category == null)
            {
                return null;
            }
            category.Name = request.Name;
            category.Icon = request.Icon ?? string.Empty;
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.Name,
                Icon = category.Icon
            };
        }

        public async Task<CategoryReadDTO?> GetCategoryByIdAsync(Guid id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryReadDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon
                })
                .FirstOrDefaultAsync();
            if(category == null)
                return null;
            return category;
        }

        public async Task<IEnumerable<CategoryReadDTO>> GetUserCategoriesAsync(Guid userId)
        {
            var categories = await _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new CategoryReadDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Icon = c.Icon
                })
                .ToListAsync();
            if (categories == null)
                return null;
            return categories;
        }


        public async Task<bool> DeleteCategoryAsync(Guid userId, Guid id)
        {
            var category = await _context.Categories
                .Where(c => c.Id == id && c.UserId == userId)
                .FirstOrDefaultAsync();
            if (category == null)
            {
                return false;
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
