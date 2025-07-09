using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using SupplyChain.DatabaseContext;
using SupplyChain.Models;

namespace Infrastructure.Data.Migrations
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        //public async Task<IReadOnlyList<ProductBrand>> GetProductBrandAsync()
        //{
        //    return await _context.ProductBrands.ToListAsync();
        //}

        //public async Task<Product> GetProductByIdAsync(int id)
        //{
        //     return await _context.Products
        //     .Include(p=>p.ProductType)
        //     .Include(p=>p.ProductBrand)
        //     .FirstOrDefaultAsync(p=>p.ProductId==id);
        //}

        //public async Task<IReadOnlyList<Product>> GetProductsAsync( )
        //{
        //   return await _context.Products.
        //   Include(p=>p.ProductType)
        //   .Include(p=>p.ProductBrand).
        //   ToListAsync();
        //}

        //public async Task<IReadOnlyList<ProductType>> GetProductTypesAsync()
        //{
        //     return await _context.ProductTypes.ToListAsync();


        //}
        public async Task AddProductAsync(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null");
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}