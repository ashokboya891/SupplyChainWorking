namespace SupplyChain.DTOs
{
    public class OrderDto
    {
      public int   OrderId{ set; get; }
        public DateTime OrderDate { set; get; }
         public decimal TotalAmount { set; get; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
