using Inventory.Core.Response;
using Inventory.Repository.Model;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Inventory.Services.IServices
{
    public interface ITokenService
    {
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        SecurityToken GenerateToken(AppUserEntity user, IList<string> userRoles);
        string GetuserId(string token);
        bool TryGetuserId(string token, out ResponseMessage result);
    }
}
