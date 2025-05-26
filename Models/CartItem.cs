using SupplyChain.DTOs;

namespace SupplyChain.Models
{
    public class Cart
    {
        public Guid UserId { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; }
    }

}
