//using StocksApi.DTO;
//using Microsoft.IdentityModel.Tokens;
//using StocksApi.DatabaseContext;
//using StocksApi.DTO;
//using StocksApi.ServiceContracts;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;
//using Microsoft.AspNetCore.Identity;

//namespace StocksApi.Services
//{
//    public class JwtService : IJwtService
//    {
//        private readonly IConfiguration _configuration;

//        public JwtService(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }
//        public AuthenticationResponse CreateJwtToken(ApplicationUser user)
//        {


//            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

//              Claim[] claims = new Claim[] {
//              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //Subject (user id)

//              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //JWT unique ID

//              new Claim(JwtRegisteredClaimNames.Iat,DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()), //Issued at (date and time of token generation)

//              new Claim(ClaimTypes.NameIdentifier, user.Email), //Unique name identifier of the user (Email)

//              new Claim(ClaimTypes.Name, user.PersonName), //Name of the user
//              new Claim(ClaimTypes.Email, user.Email) ,//email of the user

//              };


//            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(
//             Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
//             );

//            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

//            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
//             _configuration["Jwt:Issuer"],
//             _configuration["Jwt:Audience"],
//             claims,
//             expires: expiration,
//             signingCredentials: signingCredentials
//             );

//            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
//            string token = tokenHandler.WriteToken(tokenGenerator);


//            return new AuthenticationResponse() { Token = token, Email = user.Email, PersonName = user.PersonName, Expire = expiration,
//            RefreshToken = GenerateRefreshToken(),
//                RefreshTokenExpirationDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]))  //from now after 10m addd this time
//            };

//         }
//        //Creates a refresh token (base 64 string of random numbers)
//        private string GenerateRefreshToken()
//        {
//            byte[] bytes = new byte[64];
//            var randomNumberGenerator = RandomNumberGenerator.Create();
//            randomNumberGenerator.GetBytes(bytes);
//            return Convert.ToBase64String(bytes);
//        }

//        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
//        {
//            var tokenValidationParameters = new TokenValidationParameters()
//            {
//                ValidateAudience = true,
//                ValidAudience = _configuration["Jwt:Audience"],
//                ValidateIssuer = true,
//                ValidIssuer = _configuration["Jwt:Issuer"],

//                ValidateIssuerSigningKey = true,
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),

//                ValidateLifetime = false //should be false
//            };

//            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

//            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

//            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))  //if the given token is not jwt token or given token is not maching with hmacsha256 alogirtm token then it not valida one 
//            {
//                throw new SecurityTokenException("Invalid token");
//            }

//            return principal;
//        }
//    }
//}

using Microsoft.IdentityModel.Tokens;
using SupplyChain.DatabaseContext;
using SupplyChain.ServiceContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using SupplyChain.DTOs;

namespace SupplyChain.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public JwtService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<AuthenticationResponse> CreateJwtToken(ApplicationUser user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Name, user.PersonName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // ✅ Add roles
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration,
                SigningCredentials = signingCredentials,
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);

            return new AuthenticationResponse
            {
                Token = jwt,
                Email = user.Email,
                PersonName = user.PersonName,
                Expire = expiration,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDateTime = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"])),
                Roles = roles.ToList() // ✅ Return roles to frontend as well
            };
        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];
            var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}

