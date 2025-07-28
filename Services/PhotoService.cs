using API.Helpers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.IServiceContracts;
using SupplyChain.Models;

namespace SupplyChain.Services
{
    public class PhotoService: IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ApplicationDbContext _context;
        public PhotoService(IOptions<CloudinarySettings> config,ApplicationDbContext context)
        {
            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
            _context = context;
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                    Folder = "SupplyChain"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);

            }
            return uploadResult;

        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicID)
        {
            var deleteParams = new DeletionParams(publicID);
            return await _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<PhotoDto?> AddProductPhotoAndSaveAsync(int productId, IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            // Fetch product and include photos
            var product = await _context.Products
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) return null;

            // Upload to Cloudinary
            var uploadResult = new ImageUploadResult();
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "SupplyChain"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null) return null;

            // Add to product entity
            var photo = new ProductPhoto
            {
                Url = uploadResult.SecureUrl.AbsoluteUri,
                PublicId = uploadResult.PublicId
            };

            product.Photos.Add(photo);

            // Save to DB
            var saved = await _context.SaveChangesAsync() > 0;
            if (!saved) return null;

            return new PhotoDto
            {
                Id = photo.PhotoId,
                Url = photo.Url,
                IsMain = photo.IsPrimary,
                PublicId = photo.PublicId
            };
        }

    }
}
