using System.ComponentModel.DataAnnotations;

namespace SupplyChain.DTOs
{
    public class ProductPhotoUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public int Id { get; set; }
    }
}
