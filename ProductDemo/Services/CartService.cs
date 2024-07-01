using Microsoft.EntityFrameworkCore;
using ProductDemo.Data;
using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProductDemo.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddToCartAsync(Guid productId, int quantity, string configurationOption, string windowsOption, string cardOption, string description, string colorOption)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var buyerName = _httpContextAccessor.HttpContext!.User.Identity!.Name;
            var product = await _context.Product.Include(p => p.ProductCategory).FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            if (product.ProductCategory == null)
            {
                throw new ArgumentException("Product category not found");
            }

            decimal additionalPrice = 0;
            var productType = product.ProductCategory.ProductTypeName;
            decimal itemPrice = product.Price;

            if (productType == "Laptop" || productType == "Computer")
            {
                if (configurationOption == "Trung Bình" && cardOption == "Trung Bình" && windowsOption == "Trung Bình")
                {
                    additionalPrice = 500000;
                }
                else if (configurationOption == "Cao" && cardOption == "Cao" && windowsOption == "Cao")
                {
                    additionalPrice = 30000000;
                }
                else if (configurationOption == "Thấp" && cardOption == "Trung Bình" && windowsOption == "Thấp" ||
                         configurationOption == "Thấp" && cardOption == "Thấp" && windowsOption == "Trung Bình" ||
                         configurationOption == "Thấp" && cardOption == "Cao" && windowsOption == "Thấp" ||
                         configurationOption == "Thấp" && cardOption == "Thấp" && windowsOption == "Cao" ||
                         configurationOption == "Trung Bình" && cardOption == "Thấp" && windowsOption == "Thấp" ||
                         configurationOption == "Trung Bình" && cardOption == "Trung Bình" && windowsOption == "Thấp")
                {
                    additionalPrice = 1000000;
                }
                else if (configurationOption == "Thấp" && cardOption == "Cao" && windowsOption == "Trung Bình" ||
                         configurationOption == "Thấp" && cardOption == "Trung Bình" && windowsOption == "Cao" ||
                         configurationOption == "Trung Bình" && cardOption == "Trung Bình" && windowsOption == "Cao" ||
                         configurationOption == "Cao" && cardOption == "Trung Bình" && windowsOption == "Cao" ||
                         configurationOption == "Cao" && cardOption == "Cao" && windowsOption == "Trung Bình" ||
                         configurationOption == "Trung Bình" && cardOption == "Cao" && windowsOption == "Trung Bình")
                {
                    additionalPrice = 2000000;
                }
                itemPrice += additionalPrice;
            }

            var cartItem = await _context.CartItem.FirstOrDefaultAsync(c =>
                c.ProductId == productId &&
                c.UserId == userId &&
                c.Configuration == configurationOption &&
                c.WindowsOption == windowsOption &&
                c.CardOption == cardOption &&
                c.ColorOption == colorOption
            );

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.Price = itemPrice * cartItem.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = quantity,
                    Price = itemPrice * quantity,
                    AddedDate = DateTime.Now,
                    UserId = userId,
                    BuyerName = buyerName,
                    Configuration = configurationOption,
                    WindowsOption = windowsOption,
                    CardOption = cardOption,
                    ColorOption = colorOption,
                    Descriptions = description
                };
                _context.CartItem.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.CartItem
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem> GetCartItemByIdAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItem
                .Include(ci => ci.Product)
                .ThenInclude(p => p!.ProductCategory)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
            {
                throw new Exception("CartItem not found");
            }
            return cartItem;
        }

        public async Task<bool> UpdateCartItemAsync(CartItem cartItem)
        {
            _context.CartItem.Update(cartItem);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> RemoveFromCartAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItem.FindAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }
            _context.CartItem.Remove(cartItem);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<int> GetTotalPagesAsync(int pageSize)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalItems = await _context.CartItem.CountAsync(c => c.UserId == userId);
            return (int)Math.Ceiling(totalItems / (double)pageSize);
        }

        public async Task<int> GetTotalItemsAsync()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.CartItem.CountAsync(c => c.UserId == userId);
        }

        public async Task ClearCartAsync()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _context.CartItem.Where(c => c.UserId == userId).ToListAsync();
            _context.CartItem.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCartTotalAsync()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.CartItem
                .Where(ci => ci.UserId == userId)
                .SumAsync(ci => ci.Price);
        }
    }
}
