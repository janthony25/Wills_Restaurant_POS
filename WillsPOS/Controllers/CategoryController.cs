using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("test-error")]
        public IActionResult SimulateError()
        {
            throw new Exception("Simulated error for global hanlder test.");
        }
    }
}
