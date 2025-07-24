namespace SupplyChain.DTOs
{
    public class PaymentNotificationMessage
    {
        public string SenderUsername { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string OrderId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Context { set; get; }
    }
}
