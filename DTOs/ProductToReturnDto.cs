namespace SupplyChain.DTOs
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductType { get; set; }
        public string ProductBrand { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string CurrentStock { set; get; }


        public string PrimaryImageUrl { get; set; }  // ✅ For main card
        public List<string> AllImages { get; set; } = new(); // ✅ For gallery
    }
}
