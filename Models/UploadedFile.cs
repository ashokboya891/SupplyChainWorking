namespace SupplyChain.Models
{
    public class UploadedFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;
    }
}
