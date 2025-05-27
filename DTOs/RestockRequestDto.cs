namespace SupplyChain.DTOs
{
    public class RestockRequestDto
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
        public int ShortageQty { get; set; }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public int threshold { get; set; }
        public DateTime CreatedDate { set; get; }
    }
}
