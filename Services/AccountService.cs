using Microsoft.AspNetCore.Identity;
using SupplyChain.DatabaseContext;
using SupplyChain.DTO;
using SupplyChain.DTOs;
using SupplyChain.IRepoContracts;
using SupplyChain.IServiceContracts;
using SupplyChain.ServiceContracts;

namespace SupplyChain.Services
{
    public class AccountService : IAccountService
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository repo)
        {
            _accountRepository = repo;
        }

        public async Task<AuthenticationResponse> Login(LoginDTO loginDto)
        {

            if(loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto), "Login DTO cannot be null.");
            }
            else
            {
               var result=await _accountRepository.Login(loginDto).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        throw task.Exception ?? new Exception("An error occurred during login.");
                    }
                    return task.Result;
                });
                return result;
            }
        }
    }
}
