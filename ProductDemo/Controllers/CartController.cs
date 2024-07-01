using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Services;
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
        private IHttpContextAccessor _httpContextAccessor;

        public CartController(ICartService cartService, IInvoiceService invoiceService, ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _invoiceService = invoiceService;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            ViewBag.ListProducts = await _context.CartItem.Where(x => x.UserId == user!.Id).Include(x => x.Product)
                .Select(x => x.Product)
                .ToListAsync();
            var cartItems = await _cartService.GetCartItemsAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(Guid productId, int quantity, string configurationOption, string windowsOption, string cardOption, string description, string colorOption)
        {
            await _cartService.AddToCartAsync(productId, quantity, configurationOption, windowsOption, cardOption, description, colorOption);
            return RedirectToAction("Index");
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

        [HttpGet]
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid cartItemId, int quantity, string configuration, string windowsOption, string cardOption, string colorOption)
        {
            if (!ModelState.IsValid)
            {
                var existingCartItem = await _cartService.GetCartItemByIdAsync(cartItemId);
                if (existingCartItem == null)
                {
                    return NotFound();
                }
                var product = await _context.Product.FindAsync(existingCartItem.ProductId);
                if (product == null)
                {
                    return NotFound();
                }
                if (product.ProductCategory!.ProductTypeName == "Laptop" || product.ProductCategory!.ProductTypeName == "Computer")
                {
                    existingCartItem.Quantity = quantity;
                    existingCartItem.Configuration = configuration;
                    existingCartItem.WindowsOption = windowsOption;
                    existingCartItem.CardOption = cardOption;

                    decimal additionalPrice = 0;
                    if (configuration == "Trung Bình" && cardOption == "Trung Bình" && windowsOption == "Trung Bình")
                    {
                        additionalPrice = 2000000;
                    }
                    else if (configuration == "Cao" && cardOption == "Cao" && windowsOption == "Cao")
                    {
                        additionalPrice = 30000000;
                    }
                    else if (configuration == "Thấp" && cardOption == "Trung Bình" && windowsOption == "Thấp" ||
                             configuration == "Thấp" && cardOption == "Thấp" && windowsOption == "Trung Bình" ||
                             configuration == "Thấp" && cardOption == "Cao" && windowsOption == "Thấp" ||
                             configuration == "Thấp" && cardOption == "Thấp" && windowsOption == "Cao" ||
                             configuration == "Trung Bình" && cardOption == "Thấp" && windowsOption == "Thấp" ||
                             configuration == "Trung Bình" && cardOption == "Trung Bình" && windowsOption == "Thấp")
                    {
                        additionalPrice = 1000000;
                    }
                    else if (configuration == "Thấp" && cardOption == "Cao" && windowsOption == "Trung Bình" ||
                             configuration == "Thấp" && cardOption == "Trung Bình" && windowsOption == "Cao" ||
                             configuration == "Trung Bình" && cardOption == "Trung Bình" && windowsOption == "Cao" ||
                             configuration == "Cao" && cardOption == "Trung Bình" && windowsOption == "Cao" ||
                             configuration == "Cao" && cardOption == "Cao" && windowsOption == "Trung Bình" ||
                             configuration == "Trung Bình" && cardOption == "Cao" && windowsOption == "Trung Bình")
                    {
                        additionalPrice = 2000000;
                    }
                    existingCartItem.Price = (product.Price + additionalPrice) * existingCartItem.Quantity;
                }
                else
                {
                    existingCartItem.ColorOption = colorOption;
                }
                var success = await _cartService.UpdateCartItemAsync(existingCartItem);
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