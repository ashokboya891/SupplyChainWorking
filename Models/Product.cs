using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int CurrentStock { get; set; }
        public int Threshold { get; set; }
        public decimal Price { get; set; }
        public ICollection<InventoryLog> InventoryLogs { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<RestockRequest> RestockRequests { get; set; }
    }
}
