using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Planify_BackEnd.DTOs.Categories;
using Planify_BackEnd.Services.Categories;

namespace Planify_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet("{campusId}")]
        public IActionResult GetCategoryByCampusId(int campusId)
        {
            try
            {
                var response = _categoryService.GetCategoryByCampusId(campusId);
                return Ok(response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize(Roles = "Campus Manager")]
        public async Task<IActionResult> CreateCategory(CategoryDTO categoryDTO)
        {
            try
            {
                var campusClaim = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "campusId").Value);
                var response = await _categoryService.CreateCategory(categoryDTO,campusClaim);
                return StatusCode(response.Status,response);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        [Authorize(Roles = "Campus Manager")]
        public async Task<IActionResult> UpdateCategory(CategoryDTO categoryDTO)
        {
            try
            {
                var campusClaim = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "campusId").Value);
                var response = await _categoryService.UpdateCategory(categoryDTO,campusClaim);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{categoryId}")]
        [Authorize(Roles = "Campus Manager")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var response = await _categoryService.DeleteCategory(categoryId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
