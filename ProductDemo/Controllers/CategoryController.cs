using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDemo.Models;
using ProductDemo.Services;
using System;
using System.Threading.Tasks;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var viewModel = await categoryService.GetCategoriesAsync(search, page);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                await categoryService.CreateCategoryAsync(productCategory);
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                var result = await categoryService.UpdateCategoryAsync(id, productCategory);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, ProductCategory productCategory)
        {
            var result = await categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
