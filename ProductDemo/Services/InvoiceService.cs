using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;

namespace ProductDemo.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<List<ProductCategory>> GetAllCategoriesAsync()
        {
            return await _context.ProductCategory.ToListAsync();
        }

        public async Task<bool> CreateInvoiceAsync(Invoice invoice)
        {
            _context.Invoice.Add(invoice);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateInvoiceAsync(Guid id, Invoice invoice)
        {
            var existingInvoice = await _context.Invoice.FindAsync(id);
            if (existingInvoice == null) return false;

            existingInvoice.BuyerName = invoice.BuyerName;
            existingInvoice.ProductName = invoice.ProductName;
            existingInvoice.ProductCategory = invoice.ProductCategory;
            existingInvoice.Price = invoice.Price;
            existingInvoice.PurchaseDate = invoice.PurchaseDate;
            existingInvoice.Quantity = invoice.Quantity;
            existingInvoice.ProductId = invoice.ProductId;
            existingInvoice.BuyerId = invoice.BuyerId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteInvoiceAsync(Guid id)
        {
            var invoice = await _context.Invoice.FindAsync(id);
            if (invoice == null) return false;

            _context.Invoice.Remove(invoice);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
