using SupplyChain.DatabaseContext;
using System.Security.Claims;
using SupplyChain.DTOs;

namespace SupplyChain.ServiceContracts
{
    public interface IJwtService
    {
        Task<AuthenticationResponse> CreateJwtToken(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromJwtToken(string? token);
    }
}
