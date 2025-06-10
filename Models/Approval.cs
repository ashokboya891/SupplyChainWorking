using SupplyChain.Enum;

namespace SupplyChain.Models
{
    public class Approval
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public CreateRequest Request { get; set; }

        public ApprovalRole Role { get; set; }
        public string ApproverId { get; set; }
        public string Status { get; set; } // Pending, Approved, Rejected
        public DateTime Timestamp { get; set; }
    }
}
