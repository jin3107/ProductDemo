using ProductDemo.Models;
using ProductDemo.ViewModels;
using System;
using System.Threading.Tasks;

namespace ProductDemo.Services
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> GetCategoriesAsync(string search, int page);
        Task CreateCategoryAsync(ProductCategory productCategory);
        Task<ProductCategory> GetCategoryByIdAsync(Guid id);
        Task<bool> UpdateCategoryAsync(Guid id, ProductCategory productCategory);
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
