namespace SupplyChain.DTOs
{
    public class InventoryLogDto
    {
      public int LogId { set; get; }
       public DateTime   CreatedDate{ set; get; }
        public int  QuantityChange{set;get;}
        public ProductDto Product { set; get; }
        public UserDto User { set; get; }
    }
}
