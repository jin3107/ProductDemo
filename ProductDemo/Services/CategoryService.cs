using ProductDemo.Models;
using ProductDemo.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;

namespace ProductDemo.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext context;

        public CategoryService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<CategoryViewModel> GetCategoriesAsync(string search, int page)
        {
            var cacheEntry = new CategoryViewModel();
            var query = context.ProductCategory.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.ProductTypeName!.Contains(search));
            }

            cacheEntry.Categories = await query
                .OrderBy(c => c.ProductTypeName)
                .Skip((page - 1) * 5)
                .Take(5)
                .ToListAsync();

            cacheEntry.Page = page;
            cacheEntry.NoOfPages = (int)Math.Ceiling((double)await query.CountAsync() / 5);
            cacheEntry.Search = search;

            return cacheEntry;
        }

        public async Task CreateCategoryAsync(ProductCategory productCategory)
        {
            context.ProductCategory.Add(productCategory);
            await context.SaveChangesAsync();
        }

        public async Task<ProductCategory> GetCategoryByIdAsync(Guid id)
        {
            return await context.ProductCategory.AsNoTracking().FirstOrDefaultAsync(c => c.ProductTypeId == id);
        }

        public async Task<bool> UpdateCategoryAsync(Guid id, ProductCategory productCategory)
        {
            var existingCategory = await context.ProductCategory.FindAsync(id);
            if (existingCategory == null)
            {
                return false;
            }

            existingCategory.ProductTypeName = productCategory.ProductTypeName;
            context.ProductCategory.Update(existingCategory);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var category = await context.ProductCategory.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            context.ProductCategory.Remove(category);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
