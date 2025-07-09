using SupplyChain.Enum;

namespace SupplyChain.Models
{
    public class CategoryApprovalStage
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public ApprovalRole Role { get; set; }
        public int StageOrder { get; set; }
    }
}
