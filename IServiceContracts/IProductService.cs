using SupplyChain.DTOs;
using SupplyChain.Models;

namespace SupplyChain.IServiceContracts
{
    public interface IProductService
    {
        Task<int> UploadOrdersFromExcelFile(IFormFile formfile);
        Task<ProductToReturnDto> GetProductByIdAsync(int id);
        Task<bool> AddProduct(Product product);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        //Task<bool> UpdateProductAsync(Product product);
        Task<ProductMetaDataDto> GetProductMetaDataAsync();
        Task<Product> GetProductEntityByIdAsync(int id);
        Task<Product> GetProductPhotosByIdAsync(int id);


    }
}
