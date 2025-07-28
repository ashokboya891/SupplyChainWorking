using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }  // ✅ New field

        public int CurrentStock { get; set; }

        public int Threshold { get; set; }

        public decimal Price { get; set; }

        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }

        public int ProductBrandId { get; set; }
        public ProductBrand ProductBrand { get; set; }

        public ICollection<InventoryLog> InventoryLogs { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<RestockRequest> RestockRequests { get; set; }
        public ICollection<ProductPhoto> Photos { get; set; } = new List<ProductPhoto>();

        //[Key]
        //public int ProductId { get; set; }
        //public string Name { get; set; }
        //public int CurrentStock { get; set; }
        //public int Threshold { get; set; }
        //public decimal Price { get; set; }
        //public ICollection<InventoryLog> InventoryLogs { get; set; }
        //public ICollection<OrderItem> OrderItems { get; set; }
        //public ICollection<RestockRequest> RestockRequests { get; set; }
    }
}
