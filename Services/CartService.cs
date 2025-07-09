using Microsoft.Extensions.Caching.Distributed;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;
using System.Text.Json;

namespace SupplyChain.Services
{
    public class CartService : ICartService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CartService> _logger;

        public CartService(IDistributedCache cache, ILogger<CartService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<Cart> GetCartAsync(Guid userId)
        {
            var data = await _cache.GetStringAsync($"cart:{userId}");
            if (string.IsNullOrEmpty(data)) return new Cart { UserId = userId };

            return JsonSerializer.Deserialize<Cart>(data) ?? new Cart { UserId = userId };
        }

        public async Task AddOrUpdateItemAsync(Guid userId, CartItem item)
        {
            var cart = await GetCartAsync(userId);

            var existing = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            var json = JsonSerializer.Serialize(cart);
            await _cache.SetStringAsync($"cart:{userId}", json);
        }

        public async Task ClearCartAsync(Guid userId)
        {
            await _cache.RemoveAsync($"cart:{userId}");
        }

        public async Task RemoveItemFromCart(Guid userId, int itemId)
        {
            var cart = await GetCartAsync(userId);
            var itemToRemove = cart.Items.FirstOrDefault(i => i.ProductId == itemId);
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
            }
            var json = JsonSerializer.Serialize(cart);
            await _cache.SetStringAsync($"cart:{userId}", json);
        }
        
        public async Task UpdateQuantity(Guid userId, int itemId, int quantity)
        {
            var cartitems=await GetCartAsync(userId);
            var itemtoupdate=cartitems.Items.FirstOrDefault(i=>i.ProductId==itemId);
            if(itemtoupdate!=null)
            {
                itemtoupdate.Quantity=quantity;
            }
            var json = JsonSerializer.Serialize(cartitems);
            await _cache.SetStringAsync($"cart:{userId}", json);
        }
    }
}
