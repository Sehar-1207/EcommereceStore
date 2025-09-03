using Authentication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.Infrastructure.Data.Repositories
{
    public class TokenService : IToken
    {
        private readonly IConfiguration _config;
        private readonly UserManager<Appuser> _userManager;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config, UserManager<Appuser> userManager)
        {
            _config = config;
            _userManager = userManager;
            // Create the security key once and reuse it
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        }

        public async Task<string> CreateAccessTokenAsync(Appuser user)
        {
            // 1. Get the user's roles from the UserManager
            var roles = await _userManager.GetRolesAsync(user);

            // 2. Create the list of claims to be included in the token
            var claims = new List<Claim>
            {
                // Standard claims (pre-defined names)
                new Claim(JwtRegisteredClaimNames.NameId, user.Id), // The user's unique ID
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),

                // Custom claims (you can name them whatever you want)
                new Claim("fullName", user.Fullname),
                new Claim("address", user.Address)
            };

            // Add all the user's roles to the claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 3. Create signing credentials with a secure algorithm
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Describe the token's properties (claims, expiry, etc.)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Token will be valid for 7 days
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            // 5. Create the token handler and generate the token string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}