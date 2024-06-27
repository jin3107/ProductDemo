using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ApplicationDbContext context;

        public ProductController(IProductService productService, ApplicationDbContext context)
        {
            this.productService = productService;
            this.context = context;
        }

        // GET: Product
        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var viewModel = await productService.GetProductsAsync(search, page);
            return View(viewModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewBag.ListType = context.ProductCategory.Select(x => new ProductCategory
            {
                ProductTypeId = x.ProductTypeId,
                ProductTypeName = x.ProductTypeName,
            }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await productService.CreateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Detail
        public async Task<IActionResult> Detail(Guid id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Product/Edit
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await context.ProductCategory.ToListAsync();
            ViewBag.ProductCategories = new SelectList(categories, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductId,ProductName,Price,Manufactor,MadeIn,Material,ProductTypeId,RemainingNumber")] Product product)
        {
            if (ModelState.IsValid)
            {
                var success = await productService.UpdateProductAsync(id, product);
                if (!success)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(context.ProductCategory, "ProductTypeId", "ProductTypeName", product.ProductTypeId);
            ViewBag.ListType = await context.ProductCategory.Select(x => new ProductCategory
            {
                ProductTypeId = x.ProductTypeId,
                ProductTypeName = x.ProductTypeName,
            }).ToListAsync();
            return View(product);
        }

        // GET: Product/Delete
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Product pro)
        {
            var success = await productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
