using Ingenious.Models.Helpers;
using Ingenious.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ingenious.Repositories;

namespace Ingenious.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories(int? categoryId = null)
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesAsync(categoryId);
                return Ok(new ApiResponse<IEnumerable<CategoryDto>>(categories, "Categories retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while retrieving categories: {ex.Message}", 500));
            }
        }

        [HttpPost("UpsertCategory")]
        public async Task<IActionResult> UpsertCategory([FromBody] UpsertCategoryDto categoryDto)
        {
            try
            {
                if (categoryDto == null)
                {
                    return BadRequest(new ApiResponse<string>("Invalid category data.", 400));
                }

                var id = await _categoryRepository.UpsertCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategories), new { id }, new ApiResponse<int>(id, "Category added successfully.", 201));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while adding the category: {ex.Message}", 500));
            }
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new ApiResponse<string>("Invalid category ID.", 400));
                }

                await _categoryRepository.DeleteCategoryAsync(id);
                return Ok(new ApiResponse<string>("Category deleted successfully.", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"An error occurred while deleting the category: {ex.Message}", 500));
            }
        }
    }
}
