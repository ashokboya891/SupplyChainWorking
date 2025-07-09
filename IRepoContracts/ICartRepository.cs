using SupplyChain.Models;

namespace SupplyChain.IRepoContracts
{
    public interface ICartRepository
    {
        Task<Cart> GetCartAsync(Guid userId);
        Task AddOrUpdateItemAsync(Guid userId, CartItem item);
        Task ClearCartAsync(Guid userId);
    }
}
