using WillsPOS.Models;
using WillsPOS.Models.Dto;

namespace WillsPOS.Repository.IRepository
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDto>> CategoryListAsync();
        Task<CategoryDto> GetCategoryByIdAsync(int id);
        Task<CategoryDto> AddCategoryAsync(CategoryDto dto);
        Task UpdateCategoryAsync(int id, CategoryDto dto);
        Task DeleteCategoryAsync(int id);
    }
}
