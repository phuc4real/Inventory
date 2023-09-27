using Inventory.Core.Configurations;
using Inventory.Model.Entity;
using Inventory.Service.Common;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory.Service.Implement
{
    public class TokenService : ITokenService
    {
        #region Ctor & Field

        private readonly JwtConfig _config;
        public TokenService(IOptionsSnapshot<JwtConfig> config)
        {
            _config = config.Value;
        }

        #endregion

        #region Method

        public SecurityToken GenerateToken(AppUser user, List<string> userRoles)
        {
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(ClaimTypes.NameIdentifier, user.Id!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            userRoles.ForEach(x => claims.Add(new Claim(ClaimTypes.Role, x)));
            //foreach (var role in userRoles)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, role));
            //}

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey));

            var token = new JwtSecurityToken(
                 audience: _config.Audience,
                 issuer: _config.Issuer,
                 expires: DateTime.UtcNow.AddMinutes(_config.ExpireMinutes),
                 claims: claims,
                 signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256));

            return token;
        }

        public string? GetUserId(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var principal = GetPrincipalFromToken(token);

            return principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        #endregion

        #region Private

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenValidateParameter = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _config.Issuer,
                ValidateAudience = true,
                ValidAudience = _config.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidateParameter, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Cannot get principal, invalid access token");
            }

            return principal;
        }

        #endregion
    }
}
