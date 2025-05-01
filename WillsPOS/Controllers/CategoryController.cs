using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using WillsPOS.Models.Dto;
using WillsPOS.Repository.IRepository;

namespace WillsPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryRepository.CategoryListAsync();
            return Ok(categories);
        }

        [HttpGet("category-datail/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound("Category not found.");

            return Ok(category);
        }

        [HttpPost("add-category")]
        public async Task<IActionResult> AddCategory(CategoryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.CategoryName))
                return BadRequest("Invalid category data.");

            try
            {
                var createdDto = await _categoryRepository.AddCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdDto.CategoryId }, createdDto);
            }
            catch (Exception ex) when (ex.Message.Contains("already exists."))
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("update-category/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            if (dto == null || id != dto.CategoryId)
                return BadRequest("Invalid request Id. mismatch.");

            await _categoryRepository.UpdateCategoryAsync(id, dto);
            return Ok("Category updated successfully.");
        }

        [HttpDelete("delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryRepository.DeleteCategoryAsync(id);
                return Ok("Category deleted successfully.");
            }
            catch(Exception ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(ex.Message);
            }

        }
    }
}
