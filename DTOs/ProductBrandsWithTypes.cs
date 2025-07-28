using SupplyChain.Models;

namespace SupplyChain.DTOs
{
    public class BrandDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class TypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ProductMetaDataDto
    {
        public List<BrandDto> Brands { get; set; }
        public List<TypeDto> Types { get; set; }
    }

}
