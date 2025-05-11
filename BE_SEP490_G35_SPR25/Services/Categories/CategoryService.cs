using Planify_BackEnd.DTOs;
using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Categories;

namespace Planify_BackEnd.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public IEnumerable<CategoryViewModel> GetCategoryByCampusId(int campusId)
        {
            try
            {
                var list = _categoryRepository.GetCategoryByCampusId(campusId);
                if (list== null) { throw new Exception(); }
                List<CategoryViewModel> listVM =  list.Select(c => new CategoryViewModel
                {
                    Id= c.Id,
                    CampusId= c.CampusId,
                    CategoryEventName = c.CategoryEventName,
                    Status = c.Status,
                }).ToList();
                return listVM;
            }catch(Exception ex)
            {
                Console.WriteLine("category service - get category by campus: " + ex.Message);
                return new List<CategoryViewModel>();
            }
        }

        public async Task<CategoryViewModel> GetCategoryByName(string categoryName, int campusId)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryByName(categoryName, campusId);
                CategoryViewModel categoryVM = new CategoryViewModel
                {
                    Id = category.Id,
                    CampusId = category.CampusId,
                    CategoryEventName = category.CategoryEventName,
                    Status = category.Status,
                };
                return categoryVM;
            }
            catch
            {
                return new CategoryViewModel();
            }
        }

        public async Task<ResponseDTO> CreateCategory(CategoryDTO categoryDTO, int campusId)
        {
            try
            {
                if(string.IsNullOrEmpty(categoryDTO.CategoryEventName.Trim()))
                {
                    return new ResponseDTO(400, "Tên không được để trống", categoryDTO);
                }
                var c = await _categoryRepository.GetCategoryByName(categoryDTO.CategoryEventName,campusId);
                if (c!=null)
                {
                    return new ResponseDTO(400, "Loại sự kiện này đã tồn tại", categoryDTO);
                }

                CategoryEvent category = new CategoryEvent
                {
                    Id = categoryDTO.Id,
                    CategoryEventName = categoryDTO.CategoryEventName,
                    CampusId = categoryDTO.CampusId,
                    Status = 1
                };
                var createdCategory = await _categoryRepository.CreateCategory(category);
                CategoryViewModel categoryVm = new CategoryViewModel
                {
                    Id = categoryDTO.Id,
                    CategoryEventName = categoryDTO.CategoryEventName,
                    CampusId = categoryDTO.CampusId,
                    Status = 1
                };
                return new ResponseDTO(200, "Loại sự kiện mới được thêm thành công!", categoryVm);
            }catch (Exception ex)
            {
                return new ResponseDTO(400, "Lỗi xảy ra khi cập nhật danh mục! " + ex.Message, null);
            }
        }
        public async Task<ResponseDTO> UpdateCategory(CategoryDTO categoryDTO, int campusId)
        {

            if (string.IsNullOrEmpty(categoryDTO.CategoryEventName.Trim()))
            {
                return new ResponseDTO(400, "Tên không được để trống", categoryDTO);
            }
            var c = await _categoryRepository.GetCategoryByName(categoryDTO.CategoryEventName, campusId);
            if (c != null)
            {
                return new ResponseDTO(400, "Loại sự kiện này đã tồn tại", categoryDTO);
            }
            try
            {
                CategoryEvent category = new CategoryEvent
                {
                    Id = categoryDTO.Id,
                    CategoryEventName = categoryDTO.CategoryEventName,
                    CampusId = categoryDTO.CampusId,
                    Status = 1
                };
                var updatedCategory = await _categoryRepository.UpdateCategory(category);
                CategoryViewModel categoryVm = new CategoryViewModel
                {
                    Id = categoryDTO.Id,
                    CategoryEventName = categoryDTO.CategoryEventName,
                    CampusId = categoryDTO.CampusId,
                    Status = 1
                };
                return new ResponseDTO(200, "Cập nhật thành công!", categoryVm);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(400, "Lỗi xảy ra khi cập nhật danh mục! " + ex.Message, null);
            }
        }
        public async Task<ResponseDTO> DeleteCategory(int categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new ResponseDTO(400, "Danh mục không tồn tại", false);
                }
                if (category.Events.Count != 0)
                {
                    return new ResponseDTO(400, "Đang có sự kiện tồn tại với loại sự kiện này!", false);
                }
                var isDeleted = await _categoryRepository.DeleteCategory(categoryId);
                if (!isDeleted) return new ResponseDTO(400,"Xóa không thành công!",false);
                return new ResponseDTO(200, "Xóa không thành công!", false); 
            }
            catch (Exception ex)
            {
                return new ResponseDTO(400, "Lỗi xảy ra khi xóa! "+ex.Message, false);
            }
        }
    }
}
