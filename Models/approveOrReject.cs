using SupplyChain.Enum;

namespace SupplyChain.Models
{
    public class approveOrReject
    {
        public int requestId { set; get; }
        public ApprovalRole role { set; get; }
        public string action { set; get; }
        public string comment { set;get; } = string.Empty; // Optional comment, can be empty if not provided
    }
}
