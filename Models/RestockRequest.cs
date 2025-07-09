using SupplyChain.DatabaseContext;
using SupplyChain.Enum;
using System.ComponentModel.DataAnnotations;

namespace SupplyChain.Models
{
    public class RestockRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int ProductId { get; set; }
        public int ShortageQty { get; set; }
        public int Threshold { get; set; }

        public Guid AdminId { get; set; } // Admin who will approve
                                          // public RestockStatus Status { get; set; }        // Pending, Approved, Completed

        public string Status { get; set; }      

        public DateTime CreatedDate { get; set; }

        public Product Product { get; set; }
        public ApplicationUser Admin { get; set; }
    }
}
