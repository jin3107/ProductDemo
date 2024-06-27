using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductDemo.Models;
using ProductDemo.Services;
using System;
using System.Threading.Tasks;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var success = await _invoiceService.CreateInvoiceAsync(invoice);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Unable to create invoice.");
            }
            return View(invoice);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var success = await _invoiceService.UpdateInvoiceAsync(id, invoice);
                if (success)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Unable to update invoice.");
            }
            return View(invoice);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }
            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id, Invoice invoice)
        {
            var success = await _invoiceService.DeleteInvoiceAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
