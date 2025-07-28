using System.ComponentModel.DataAnnotations;

namespace SupplyChain.DTOs
{
    public class ProductWithPhotosDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public int ProductTypeId { get; set; }
        public int ProductBrandId { get; set; }
    }
}
