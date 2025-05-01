using BlogReact.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Identity.Client;
using WillsPOS.Models;
using WillsPOS.Models.Dto;
using WillsPOS.Repository.IRepository;

namespace WillsPOS.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        async Task ICategoryRepository.AddCategoryAsync(CategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.CategoryName
            };

            try
            {
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

            }
            catch(DbUpdateException ex) when (ex.InnerException?.Message.Contains("duplicate") == true)
            {
                throw new Exception("Category already exists.");
            }
        }

        async Task<List<CategoryDto>> ICategoryRepository.CategoryListAsync()
        {
            return await _context.Categories
                    .Select(c => new CategoryDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToListAsync();
        }

        async Task ICategoryRepository.DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new Exception("Category not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        async Task<CategoryDto> ICategoryRepository.GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };
        }

        async Task ICategoryRepository.UpdateCategoryAsync(CategoryDto dto)
        {
            var category = await _context.Categories.FindAsync(dto.CategoryId);

            if (category == null)
                throw new Exception("Category not found.");

            category.CategoryName = dto.CategoryName;
            await _context.SaveChangesAsync();
        }
    }
}
