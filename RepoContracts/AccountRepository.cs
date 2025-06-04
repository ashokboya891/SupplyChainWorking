using Microsoft.AspNetCore.Identity;
using SupplyChain.DatabaseContext;
using SupplyChain.DTO;
using SupplyChain.DTOs;
using SupplyChain.IRepoContracts;
using SupplyChain.ServiceContracts;

namespace SupplyChain.RepoContracts
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration config, IJwtService jwtService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _jwtService = jwtService;
            _context = context;
        }

        public async Task<AuthenticationResponse> Login(LoginDTO loginDto)
        {
       
            var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null)
                {
                    throw new Exception("User not found after successful login.");
                }

                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);

                var authenticationResponse = await _jwtService.CreateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);
                authenticationResponse.Roles = roles.ToList();
                user.UserName = user.UserName;
                user.RefreshToken = authenticationResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(user);
                //Log.Warning("Login succeeded but user record not found. Email: {Email}", loginDTO.Email);
                return authenticationResponse;
            }

            else
            {
                //Log.Warning("Login failed for Email: {Email}", loginDTO.Email);
               throw new ArgumentNullException("Invalid email or password");
            }

        }
    }
}
