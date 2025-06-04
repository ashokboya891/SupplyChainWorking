using SupplyChain.DTO;
using SupplyChain.DTOs;

namespace SupplyChain.IRepoContracts
{
    public interface IAccountRepository
    {
        Task<AuthenticationResponse> Login(LoginDTO loginDto);

    }
}
