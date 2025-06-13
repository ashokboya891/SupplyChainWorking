using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.DTO;
using SupplyChain.ServiceContracts;
using System.Security.Claims;
//using Serilog;
using SupplyChain.Models;
using SupplyChain.DatabaseContext;
using SupplyChain.DTOs;
using SupplyChain.ServiceContracts;
using SupplyChain.Enum;
using SupplyChain.IServiceContracts;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using SupplyChain.Errors;

namespace SupplyChain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration config, IJwtService service,ApplicationDbContext context,IAccountService Accservice)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _jwtService = service;
            this._context = context;
             this._accountService = Accservice;
        }
        [HttpGet("logout")]
        public async Task<IActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync(); //it will remove identity cookie from developer tool in browser so as long as cookie preset in chrome it will consider as loged in account
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            // Validate email
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return Unauthorized(new ApiResponse(401, "Invalid email or password"));

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401, "Invalid email or password"));

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Create JWT + refresh token
            var authResponse = await _jwtService.CreateJwtToken(user);
            authResponse.Email = user.Email;
            authResponse.Roles = roles.ToList();

            // Save refresh token info to DB
            user.RefreshToken = authResponse.RefreshToken;
            user.RefreshTokenExpirationDateTime = authResponse.RefreshTokenExpirationDateTime;
            await _userManager.UpdateAsync(user);

            return Ok(authResponse);
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> PostLogin([FromBody] LoginDTO loginDTO)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDTO.Email);
        //    if (user == null) return Unauthorized(new ApiResponse(401));
        //    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
        //    if (!result.Succeeded) return Unauthorized(new ApiResponse(401));
        //    var roles = await _userManager.GetRolesAsync(user);

        //    var twotokens = await _jwtService.CreateJwtToken(user);
        //    return new AuthenticationResponse
        //    {
        //        Email = user.Email,
        //        Token = await _jwtService.CreateJwtToken(user),
        //        Roles =roles.ToList(),

        //    };
        // if (ModelState.IsValid == false)
        // {
        //     string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        //     return Problem(errorMessage);
        // }
        //var response=await _accountService.Login(loginDTO).ContinueWith(task =>
        // {
        //     if (task.IsFaulted)
        //     {
        //         //return BadRequest();
        //         throw task.Exception ?? new Exception("An error occurred during login.");
        //     }
        //     return task.Result;
        // });
        // return Ok(response);
        //var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

        //if (result.Succeeded)
        //{
        //    ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

        //    if (user == null)
        //    {
        //        return NoContent();
        //    }

        //    await _signInManager.SignInAsync(user, isPersistent: false);

        //    var authenticationResponse =await _jwtService.CreateJwtToken(user);
        //    var roles = await _userManager.GetRolesAsync(user);
        //    authenticationResponse.Roles = roles.ToList();
        //    user.UserName = user.UserName;
        //    user.RefreshToken = authenticationResponse.RefreshToken;
        //    user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
        //    await _userManager.UpdateAsync(user);
        //    return Ok(authenticationResponse);
        //}

        //else
        //{
        //    return Problem("Invalid email or password");
        //}

        //}
        [HttpPost("Register")]
        public async Task<IActionResult> PostRegister([FromBody] RegisterDTO registerDTO)
        {
            //Validation
            if (ModelState.IsValid == false)
            {
                string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Problem(errorMessage);
            }

            if (CheckEmailExistsAsync(registerDTO.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse { Errors = new[] { "Email Address is in Use" } });
            }

            //Create user
            ApplicationUser user = new ApplicationUser()
            {
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.Phone,
                UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName,

            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
            // Assign the "User" role
            var roleExist = await _roleManager.RoleExistsAsync(UserTypeOptions.User.ToString());
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new ApplicationRole { Name = UserTypeOptions.User.ToString() });
            }
            await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());

            if (result.Succeeded)
            {
                //sign-in
                await _signInManager.SignInAsync(user, isPersistent: false);

                var roles = await _userManager.GetRolesAsync(user);

                // Create JWT + refresh token
                var authResponse = await _jwtService.CreateJwtToken(user);
                authResponse.Email = user.Email;
                authResponse.Roles = roles.ToList();

                // Save refresh token info to DB
                user.RefreshToken = authResponse.RefreshToken;
                user.RefreshTokenExpirationDateTime = authResponse.RefreshTokenExpirationDateTime;
                //var authenticationResponse =await _jwtService.CreateJwtToken(user);
                //var roles = await _userManager.GetRolesAsync(user);
                //authenticationResponse.Roles = roles.ToList();
                //user.RefreshToken = authenticationResponse.RefreshToken;
                //user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;
                await _userManager.UpdateAsync(user);

                return Ok(authResponse);
            }
            else
            {
                string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description)); //error1 | error2
                return Problem(errorMessage);
            }
        }
        private async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Ok(true);  //valid mail to register this email
            }
            else
            {
                return Ok(false);  //already present email in db
            }

        }
        [HttpPost("generate-new-jwt-token")]
        public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
        {
            if (tokenModel == null)
            {
                return BadRequest("Invalid client request");
            }

            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(tokenModel.Token);  //payload data from clinet will be there inside pricipal
            if (principal == null)
            {
                return BadRequest("Invalid jwt access token");
            }

            string? email = principal.FindFirstValue(ClaimTypes.Email);

            ApplicationUser? user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpirationDateTime <= DateTime.Now)
            {
                return BadRequest("Invalid refresh token");
            }

            AuthenticationResponse authenticationResponse =await  _jwtService.CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            authenticationResponse.Roles = roles.ToList();
            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpirationDateTime = authenticationResponse.RefreshTokenExpirationDateTime;

            await _userManager.UpdateAsync(user);

            return Ok(authenticationResponse);
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}
