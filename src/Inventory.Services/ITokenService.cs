using Inventory.Model.Entity;
using Inventory.Service.Common;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Inventory.Service
{
    public interface ITokenService
    {
        public SecurityToken GenerateToken(AppUser user, List<string> userRoles);
        public string? GetUserId(string token);
    }
}
