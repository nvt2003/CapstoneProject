using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Categories
{
    public interface ICategoryRepository
    {
        Task<CategoryEvent> GetByIdAsync(int CategoryEventId);
        IEnumerable<CategoryEvent> GetCategoryByCampusId(int campusId);
        Task<CategoryEvent?> GetCategoryByName(string categoryName, int campusId);
        Task<CategoryEvent> CreateCategory(CategoryEvent categoryEvent);
        Task<CategoryEvent> UpdateCategory(CategoryEvent categoryEvent);
        Task<bool> DeleteCategory(int categoryId);
    }
}
