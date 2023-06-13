using Inventory.Repository.Model;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface ITokenService
    {
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        SecurityToken GenerateToken(AppUser user, IList<string> userRoles, int expireMinutes);
        string GetUserId(string token);
    }
}
