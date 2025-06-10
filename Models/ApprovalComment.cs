using SupplyChain.Enum;

namespace SupplyChain.Models
{
    public class ApprovalComment
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public ApprovalRole Role { get; set; }
        public string ApproverId { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
