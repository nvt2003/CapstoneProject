using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Services.Categories
{
    public interface ICategoryService
    {
        IEnumerable<CategoryViewModel> GetCategoryByCampusId(int campusId);
        Task<CategoryViewModel> GetCategoryByName(string categoryName, int campusId);
        Task<ResponseDTO> CreateCategory(CategoryDTO categoryDTO,int campusId);
        Task<ResponseDTO> UpdateCategory(CategoryDTO categoryDTO,int campusId);
        Task<ResponseDTO> DeleteCategory(int categoryId);
    }
}
