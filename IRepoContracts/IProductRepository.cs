using Microsoft.AspNetCore.Mvc;
using SupplyChain.DTOs;
using SupplyChain.Models;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        //Task<Product> GetProductByIdAsync(int id);
        //Task<IReadOnlyList<Product>> GetProductsAsync( );
        
        //Task<IReadOnlyList<ProductBrand>> GetProductBrandAsync( );
        //Task<IReadOnlyList<ProductType>> GetProductTypesAsync( );
        Task AddProductAsync(Product product);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProduct(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(Product product);
        Task<bool> SaveChangesAsync();

    }
}