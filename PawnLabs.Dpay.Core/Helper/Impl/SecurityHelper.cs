using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PawnLabs.Dpay.Core.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PawnLabs.Dpay.Core.Helper.Impl
{
    public class SecurityHelper : ISecurityHelper
    {
        private TokenConfiguration tokenConfiguration;

        public SecurityHelper(IOptions<TokenConfiguration> tokenConfiguration)
        {
            this.tokenConfiguration = tokenConfiguration.Value;
        }

        public string GenerateToken(string email)
        {
            var claims = new List<Claim>() { new Claim("Email", email)};

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenConfiguration.Issuer,
                Audience = tokenConfiguration.Audience,
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(tokenConfiguration.ExpireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenConfiguration.SecretKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var securityTokenValue = tokenHandler.WriteToken(securityToken);

            return securityTokenValue;
        }
    }
}
