using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface IAuthService
    {
        Task<ResultResponse<TokenModel>> SignInAsync(LoginDTO dto);
        Task<ResultResponse<TokenModel>> SignUpAsync(RegisterDTO dto);
        Task<ResultResponse<TokenModel>> ExternalLoginAsync();
        Task<ResultResponse<TokenModel>> SignOutAsync(string token);
        Task<ResultResponse<TokenModel>> RefreshToken(string accessToken, string refreshToken);
        Task<ResultResponse<AppUserDTO>> GrantPermission(GrantRoleDTO dto);
        AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl);
    }
}
