namespace SupplyChain.DTOs
{
    public class ProductQueryParams
    {
        public int? TypeId { get; set; }
        public int? BrandId { get; set; }
        public string? Search { get; set; }
        public string? Sort { get; set; } = "name"; // default
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
