namespace SupplyChain.Models
{
    public class Request
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FileName { get; set; }  // e.g., unique filename stored
        public string OriginalFileName { get; set; } // e.g., "BulkOrder.xlsx"
        public string Status { get; set; } // Pending, Approved, Rejected
        public List<Approval> Approvals { get; set; } = new();
        public List<ApprovalComment> Comments { get; set; } = new();
    
    }
}
