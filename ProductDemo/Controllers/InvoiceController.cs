using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductDemo.Models;
using ProductDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;

        public InvoiceController(IInvoiceService invoiceService, IProductService productService)
        {
            _invoiceService = invoiceService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var viewModel = await _invoiceService.GetInvoicesAsync(search, page);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.ListType = await GetProductCategorySelectList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var success = await _invoiceService.CreateInvoiceAsync(invoice);
                if (success) return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Unable to create invoice.");
            }
            ViewBag.ListType = await GetProductCategorySelectList();
            return View(invoice);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();

            ViewBag.ListType = await GetProductCategorySelectList();
            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Invoice invoice)
        {
            if (!ModelState.IsValid)
            {
                var success = await _invoiceService.UpdateInvoiceAsync(id, invoice);
                if (success) return RedirectToAction(nameof(Index));

                ModelState.AddModelError(string.Empty, "Unable to update invoice.");
            }
            ViewBag.ListType = await GetProductCategorySelectList();
            return View(invoice);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null) return NotFound();

            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Invoice invoice)
        {
            var success = await _invoiceService.DeleteInvoiceAsync(id);
            if (!success) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        private async Task<SelectList> GetProductCategorySelectList()
        {
            var categories = await _productService.GetAllCategoriesAsync();
            return new SelectList(categories, "ProductTypeId", "ProductTypeName");
        }

        [HttpPost]
        public async Task<IActionResult> Buy(Guid productId, int quantity)
        {
            var buyerId = User.Identity?.Name;
            if (string.IsNullOrEmpty(buyerId))
            {
                return Unauthorized();
            }
            var success = await _invoiceService.BuyProductAsync(productId, quantity, buyerId);
            if (!success)
            {
                return BadRequest("Could not create or update invoice.");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
