using SupplyChain.DTO;
using SupplyChain.DTOs;

namespace SupplyChain.IServiceContracts
{
    public interface IAccountService
    {
        Task<AuthenticationResponse> Login(LoginDTO loginDto);
    }
}
