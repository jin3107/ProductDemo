using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProductDemo.Services
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetInvoicesAsync(string search, int page);
        Task<Invoice> GetInvoiceByIdAsync(Guid id);
        Task<bool> CreateInvoiceAsync(Invoice invoice);
        Task<bool> UpdateInvoiceAsync(Guid id, Invoice invoice);
        Task<bool> DeleteInvoiceAsync(Guid id);
        Task<bool> BuyProductAsync(Guid productId, int quantity, string buyerId);
    }
}