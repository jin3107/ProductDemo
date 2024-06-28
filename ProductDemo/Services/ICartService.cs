using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDemo.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid productId, int quantity);
        Task<List<CartItem>> GetCartItemsAsync();
        Task<CartItem> GetCartItemByIdAsync(Guid cartItemId);
        Task<bool> UpdateCartItemAsync(Guid cartItemId, int quantity);
        Task RemoveFromCartAsync(Guid cartItemId);
        Task ClearCartAsync();
        Task<decimal> GetCartTotalAsync();
    }
}
