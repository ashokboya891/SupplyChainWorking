using Microsoft.AspNetCore.Http.HttpResults;
using SupplyChain.DatabaseContext;
using SupplyChain.IRepoContracts;
using SupplyChain.Models;

namespace SupplyChain.RepoContracts
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task AddProductAsync(Product product)
        {
            if(product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

    }
}
