using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ProductDemo.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InvoiceService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvoiceService(ApplicationDbContext context, ILogger<InvoiceService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Invoice>> GetInvoicesAsync(string search, int page)
        {
            int pageSize = 10;
            return await _context.Invoice
                .Include(i => i.ProductCategory)
                .Where(i => (i.ProductName != null && i.ProductName.Contains(search)) ||
                            (i.BuyerName != null && i.BuyerName.Contains(search)))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceByIdAsync(Guid id)
        {
            return await _context.Invoice
                .Include(i => i.ProductCategory)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task<bool> CreateInvoiceAsync(Invoice invoice)
        {
            _context.Invoice.Add(invoice);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInvoiceAsync(Guid id, Invoice invoice)
        {
            var existingInvoice = await _context.Invoice.FirstOrDefaultAsync(i => i.InvoiceId == id);
            if (existingInvoice != null)
            {
                existingInvoice.BuyerName = invoice.BuyerName;
                existingInvoice.ProductName = invoice.ProductName;
                existingInvoice.ProductCategoryId = invoice.ProductCategoryId;
                existingInvoice.PurchaseDate = invoice.PurchaseDate;
                existingInvoice.Quantity = invoice.Quantity;

                var product = await _context.Product.FindAsync(existingInvoice.ProductId);
                if (product != null)
                {
                    existingInvoice.Price = product.Price * existingInvoice.Quantity;
                }
                try
                {
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating invoice.");
                }
            }
            return false;
        }


        public async Task<bool> DeleteInvoiceAsync(Guid id)
        {
            var invoice = await _context.Invoice.FindAsync(id);
            if (invoice == null) return false;

            _context.Invoice.Remove(invoice);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> BuyProductAsync(Guid productId, int quantity, string buyerId)
        {
            var product = await _context.Product
                .Include(p => p.ProductCategory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                _logger.LogError("Product not found");
                return false;
            }

            var buyerName = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? buyerId;
            var existingInvoice = await _context.Invoice
                .FirstOrDefaultAsync(i => i.ProductId == productId && i.BuyerId == buyerId);

            if (existingInvoice != null)
            {
                existingInvoice.Quantity += quantity;
                existingInvoice.Price = existingInvoice.Quantity * product.Price;
                _context.Invoice.Update(existingInvoice);
            }
            else
            {
                var invoice = new Invoice
                {
                    InvoiceId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductCategoryId = product.ProductCategory?.ProductTypeId ?? Guid.Empty,
                    ProductCategoryName = product.ProductCategory?.ProductTypeName ?? "Unknown",
                    Price = product.Price * quantity,
                    PurchaseDate = DateTime.Now,
                    Quantity = quantity,
                    BuyerId = buyerId,
                    BuyerName = buyerName
                };
                _context.Invoice.Add(invoice);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
