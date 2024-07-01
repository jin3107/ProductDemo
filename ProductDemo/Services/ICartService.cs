using ProductDemo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductDemo.Services
{
    public interface ICartService
    {
        Task AddToCartAsync(Guid productId, int quantity, string configurationOption, string windowsOption, string cardOption, string description, string colorOption);
        Task<List<CartItem>> GetCartItemsAsync();
        Task<CartItem> GetCartItemByIdAsync(Guid cartItemId);
        Task<bool> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> RemoveFromCartAsync(Guid cartItemId);
        Task ClearCartAsync();
        Task<decimal> GetCartTotalAsync();
    }
}
