namespace SupplyChain.DTOs
{
    public class OrderDto
    {
      public int   OrderId{ set; get; }
        public Guid UserId { set; get; }
        //public ApplicationUser User { get; set; } // Uncomment if you need user details in the DTO
        //public string UserName { get; set; } // Optional: If you want to include username in the DTO
        public DateTime OrderDate { set; get; }
         public decimal TotalAmount { set; get; }
        public string RazorpayOrderId { get; set; }
        public string PaymentStatus { get; set; }
        public string RazorpayPaymentId { get; set; }
        public DateTime? PaidAt { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
