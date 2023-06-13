using Inventory.Core.Options;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class TokenService : ITokenService
    {
        private readonly JWTOption _option;
        public TokenService(IOptionsSnapshot<JWTOption> option)
        {
            _option = option.Value;
        }
        public SecurityToken GenerateToken(AppUser user, IList<string> userRoles, int expireMinutes)
        {
            var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_option.SecretKey));

            var token = new JwtSecurityToken(
                 audience: _option.ValidAudience,
                 issuer: _option.ValidIssuer,
                 expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                 claims: authClaims,
                 signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                 );

            return token;
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidateParameter = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _option.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = _option.ValidAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_option.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidateParameter, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg
                    .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
