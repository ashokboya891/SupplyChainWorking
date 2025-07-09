namespace SupplyChain.DTOs
{
    public class StockFillDto
    {
        public int RequestId { get; set; }
        public int Quantity { get; set; }
        public string? Status { get; set; }
        //public int RequestId { set; get; }
        //public int ProductId { set; get; }
        //public int Quantity { set; get; }
        //public Guid AdminId { set; get; }
    }
}
