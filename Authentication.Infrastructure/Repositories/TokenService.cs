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

            // Correct section -> "Authentication:Key"
            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Authentication:Key"]!)
            );
        }

        public async Task<string> CreateAccessTokenAsync(Appuser user)
        {
            // 1. Get the user's roles
            var roles = await _userManager.GetRolesAsync(user);

            // 2. Create claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? string.Empty),

                // Custom claims
                new Claim("fullName", user.Fullname ?? string.Empty),
                new Claim("address", user.Address ?? string.Empty)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // 3. Signing credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // 4. Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,

                // Correct sections -> "Authentication:Issuer" & "Authentication:Audience"
                Issuer = _config["Authentication:Issuer"],
                Audience = _config["Authentication:Audience"]
            };

            // 5. Generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
