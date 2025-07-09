using SupplyChain.Models;

namespace SupplyChain.IServiceContracts
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(Guid userId);
        Task AddOrUpdateItemAsync(Guid userId, CartItem item);
        Task ClearCartAsync(Guid userId);
        Task RemoveItemFromCart(Guid userId, int itemId);
        Task UpdateQuantity(Guid userId, int itemId, int quantity);
    }
}
