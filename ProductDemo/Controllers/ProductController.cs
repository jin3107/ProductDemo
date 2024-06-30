using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IProductService productService, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.productService = productService;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create(Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.Length > 0)
                {
                    var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    var fileName = Guid.NewGuid().ToString() + "-" + Image.FileName;
                    var filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                    product.ImagePath = "/images/" + fileName;
                }
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
        public async Task<IActionResult> Edit(Guid id, [Bind("ProductId,ProductName,ImagePath,Price,Manufactor,MadeIn,Material,ProductTypeId,RemainingNumber")] Product product, IFormFile Image)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }
                if (Image != null && Image.Length > 0)
                {
                    var uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    var fileName = Guid.NewGuid().ToString() + "-" + Image.FileName;
                    var filePath = Path.Combine(uploadDir, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Image.CopyToAsync(fileStream);
                    }
                    existingProduct.ImagePath = "/images/" + fileName;
                }
                existingProduct.ProductName = product.ProductName;
                existingProduct.Price = product.Price;
                existingProduct.Manufactor = product.Manufactor;
                existingProduct.ProductTypeId = product.ProductTypeId;
                existingProduct.MadeIn = product.MadeIn;
                existingProduct.Material = product.Material;
                existingProduct.RemainingNumber = product.RemainingNumber;

                await productService.UpdateProductAsync(id, existingProduct);
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
