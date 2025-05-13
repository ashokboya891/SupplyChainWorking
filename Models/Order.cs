using SupplyChain.DatabaseContext;
using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public Guid UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        public ApplicationUser User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
