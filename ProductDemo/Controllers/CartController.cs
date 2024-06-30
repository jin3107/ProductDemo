using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Data.Migrations;
using ProductDemo.Models;
using ProductDemo.Services;
using SQLitePCL;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IInvoiceService _invoiceService;
        private readonly ApplicationDbContext _context;

        public CartController(ICartService cartService, IInvoiceService invoiceService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _invoiceService = invoiceService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ListProducts = await _context.Product.ToListAsync();
            var cartItems = await _cartService.GetCartItemsAsync();
            return View(cartItems);
        }

        public async Task<IActionResult> AddToCart(Guid productId, int quantity)
        {
            await _cartService.AddToCartAsync(productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var cartItem = await _cartService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var cartItem = await _cartService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid cartItemId, int quantity)
        {
            if (ModelState.IsValid)
            {
                var success = await _cartService.UpdateCartItemAsync(cartItemId, quantity);
                if (!success)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            var cartItem = await _cartService.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewBag.ListType = await _context.ProductCategory
                .Select(x => new ProductCategory
                {
                    ProductTypeId = x.ProductTypeId,
                    ProductTypeName = x.ProductTypeName,
                }).ToListAsync();
            return View(cartItem);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var cartItem = await _cartService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cartItem = await _cartService.GetCartItemByIdAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            await _cartService.RemoveFromCartAsync(id);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Purchase()
        {
            var cartItems = await _cartService.GetCartItemsAsync();

            foreach (var item in cartItems)
            {
                var invoice = new Invoice
                {
                    InvoiceId = Guid.NewGuid(),
                    BuyerName = User.Identity!.Name,
                    ProductName = item.Product!.ProductName,
                    ProductCategoryName = item.Product!.ProductCategory!.ProductTypeName,
                    Price = item.Price,
                    PurchaseDate = DateTime.Now,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId,
                    ProductCategoryId = item.Product.ProductTypeId,
                    BuyerId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };

                await _invoiceService.CreateInvoiceAsync(invoice);
            }

            await _cartService.ClearCartAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
