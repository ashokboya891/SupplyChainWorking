using CloudinaryDotNet.Actions;
using SupplyChain.DTOs;

namespace SupplyChain.IServiceContracts
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicID);
        Task<PhotoDto?> AddProductPhotoAndSaveAsync(int productId, IFormFile file);
    }
}
