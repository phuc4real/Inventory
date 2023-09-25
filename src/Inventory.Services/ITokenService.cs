using Inventory.Repository.Model;
using Inventory.Service.Common;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Inventory.Service
{
    public interface ITokenService
    {
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        SecurityToken GenerateToken(AppUserEntity user, IList<string> userRoles);
        string GetuserId(string token);
        bool TryGetuserId(string token, out ResultMessage result);
    }
}
