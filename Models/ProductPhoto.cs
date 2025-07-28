using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Models
{
    public class ProductPhoto
    {
        [Key]
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; } // Optional for Cloudinary deletion
        public bool IsPrimary { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
