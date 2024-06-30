using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductDemo.Data;
using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductDemo.ViewModels;

namespace ProductDemo.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext context;

        public ProductService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<ProductViewModel> GetProductsAsync(string search, int page)
        {
            var query = context.Product
                .Include(p => p.ProductCategory)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            var products = await query
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * 5)
                .Take(5)
                .ToListAsync();

            var viewModel = new ProductViewModel
            {
                Products = products,
                Page = page,
                NoOfPages = (int)Math.Ceiling((double)await query.CountAsync() / 5),
                Search = search
            };

            return viewModel;
        }

        public async Task CreateProductAsync(Product product)
        {
            context.Product.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await context.Product
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task<bool> UpdateProductAsync(Guid id, Product product)
        {
            var existingProduct = await context.Product.FindAsync(id);
            if (existingProduct == null)
            {
                return false;
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.ImagePath = product.ImagePath;
            existingProduct.Price = product.Price;
            existingProduct.Manufactor = product.Manufactor;
            existingProduct.ProductTypeId = product.ProductTypeId;
            existingProduct.MadeIn = product.MadeIn;
            existingProduct.Material = product.Material;
            existingProduct.RemainingNumber = product.RemainingNumber;

            context.Product.Update(existingProduct);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await context.Product.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            context.Product.Remove(product);
            await context.SaveChangesAsync();
            return true;
        }

        public IEnumerable<SelectListItem> GetCategoriesSelectList()
        {
            return context.ProductCategory
                .Select(c => new SelectListItem
                {
                    Value = c.ProductTypeId.ToString(),
                    Text = c.ProductTypeName
                })
                .ToList();
        }

        public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        {
            return await context.ProductCategory.ToListAsync();
        }
    }
}
