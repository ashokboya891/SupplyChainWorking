using Microsoft.AspNetCore.Mvc;

namespace SupplyChain.DTOs
{
    public class RequestFormDto
    {
        [FromForm]
        public string Title { get; set; }

        [FromForm]
        public int CategoryId { get; set; }

        [FromForm]
        public IFormFile file { get; set; }
    }
}
