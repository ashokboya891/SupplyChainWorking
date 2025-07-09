namespace SupplyChain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CategoryApprovalStage> Stages { get; set; }
    }
}
