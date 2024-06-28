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

        public async Task AddToCartAsync(Guid productId, int quantity)
        {
            var product = await _context.Product.FindAsync(productId);
            if (product == null) throw new ArgumentException("Product not found");

            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == userId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                cartItem.Price = product.Price * cartItem.Quantity;
            }
            else
            {
                cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price * quantity,
                    AddedDate = DateTime.Now,
                    UserId = userId
                };
                _context.CartItem.Add(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.CartItem
                .Include(ci => ci.Product)
                .ThenInclude(p => p!.ProductCategory)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
        }

        public async Task<CartItem> GetCartItemByIdAsync(Guid cartItemId)
        {
            return await _context.CartItem
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
        }

        public async Task<bool> UpdateCartItemAsync(Guid cartItemId, int quantity)
        {
            var cartItem = await _context.CartItem
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

            if (cartItem != null && cartItem.Product != null)
            {
                cartItem.Quantity = quantity;
                cartItem.Price = cartItem.Product.Price * quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task RemoveFromCartAsync(Guid cartItemId)
        {
            var cartItem = await _context.CartItem.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItem.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _context.CartItem.Where(ci => ci.UserId == userId);
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
