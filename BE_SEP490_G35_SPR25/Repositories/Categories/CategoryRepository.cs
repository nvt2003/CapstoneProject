using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.Models;

namespace Planify_BackEnd.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly PlanifyContext _context;
        public CategoryRepository(PlanifyContext context)
        {
            _context = context;
        }
        public async Task<CategoryEvent> GetByIdAsync(int CategoryEventId)
        {
            try {
                return await _context.CategoryEvents
                    .Include(c=>c.Events)
                    .FirstOrDefaultAsync(c => c.Id == CategoryEventId);
            } 
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred.", ex);
            }
        }

        public IEnumerable<CategoryEvent> GetCategoryByCampusId(int campusId)
        {
            try
            {
                var list = _context.CategoryEvents.Where(c=>c.CampusId==campusId&&c.Status==1).ToList();
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("category repository - get by campus id: " + ex.Message);
                return new List<CategoryEvent>();
            }
        }

        public async Task<CategoryEvent?> GetCategoryByName(string categoryName, int campusId)
        {
            try
            {
                var category = await _context.CategoryEvents
                    .FirstOrDefaultAsync(c => c.CategoryEventName.Equals(categoryName) 
                    && c.CampusId == campusId
                    && c.Status == 1);
                return category;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<CategoryEvent> CreateCategory(CategoryEvent categoryEvent)
        {
            try
            {
                _context.Add(categoryEvent);
                await _context.SaveChangesAsync();
                return categoryEvent;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<CategoryEvent> UpdateCategory(CategoryEvent categoryEvent)
        {
            try
            {
                var category = _context.CategoryEvents.FirstOrDefault(c=>c.Id==categoryEvent.Id);
                category.CategoryEventName = categoryEvent.CategoryEventName;
                _context.Update(category);
                await _context.SaveChangesAsync();
                return category;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteCategory(int categoryId)
        {
            try
            {
                var category = _context.CategoryEvents.FirstOrDefault(c => c.Id == categoryId);
                if (category == null) throw new NullReferenceException();
                category.Status = 0;
                _context.Update(category);
                await _context.SaveChangesAsync();
                return true;
            }catch(NullReferenceException nEx)
            {
                throw new NullReferenceException(nEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
