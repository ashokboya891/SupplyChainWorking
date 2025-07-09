using SupplyChain.DatabaseContext;
using SupplyChain.Models;
using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Models
{
    public class InventoryLog
    {
        [Key]
        public int LogId { get; set; }

        public int ProductId { get; set; }
        public Guid UserId { get; set; }
        public int QuantityChange { get; set; } // +ve for restock, -ve for orders
        public string Reason { get; set; }
        public DateTime CreatedDate { get; set; }

        public Product Product { get; set; }
        public ApplicationUser User { get; set; }


    }
}
