using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using SupplyChain.IRepoContracts;
using SupplyChain.Models;
using System.Text.Json;

namespace SupplyChain.RepoContracts
{
    public class CartRepository:ICartRepository
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CartRepository> _logger;

        public CartRepository(IDistributedCache cache, ILogger<CartRepository> logger)
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
    }
}
