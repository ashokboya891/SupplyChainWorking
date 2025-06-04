using SupplyChain.Models;

namespace SupplyChain.IRepoContracts
{
    public interface IProductRepository
    {
        Task AddProductAsync(Product product);
    }
}
