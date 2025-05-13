using Microsoft.AspNetCore.Identity;
using SupplyChain.Models;

namespace SupplyChain.DatabaseContext
{
    public class ApplicationUser: IdentityUser<Guid>
    {
        public string? PersonName { set; get; }
        public string? RefreshToken { set; get; }

        public ICollection<Order> Orders { get; set; }
        public ICollection<InventoryLog> InventoryLogs { get; set; }
        public ICollection<RestockRequest> RestockRequests { get; set; }
        public DateTime RefreshTokenExpirationDateTime { get; set; }
    }
}
