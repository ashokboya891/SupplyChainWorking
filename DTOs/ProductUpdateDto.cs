namespace SupplyChain.DTOs
{
    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentStock { get; set; }
        public decimal Price { get; set; }
        public int ProductTypeId { get; set; }
        public int ProductBrandId { get; set; }
    }
}
