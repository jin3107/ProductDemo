using Microsoft.AspNetCore.Mvc.Rendering;
using ProductDemo.Models;
using ProductDemo.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDemo.Services
{
    public interface IProductService
    {
        Task<ProductViewModel> GetProductsAsync(string search, int page);
        Task CreateProductAsync(Product product);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<bool> UpdateProductAsync(Guid id, Product product);
        Task<bool> DeleteProductAsync(Guid id);
        IEnumerable<SelectListItem> GetCategoriesSelectList();
        Task<List<ProductCategory>> GetAllCategoriesAsync();
    }
}
