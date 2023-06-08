using Inventory.Core.Common;
using Inventory.Core.Response;
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
        Task<AuthResponse> SignInAsync(string username, string password);

        Task<AuthResponse> SignUpAsync(string email, string username,string password);

        AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl);

        Task<AuthResponse> ExternalLoginAsync();

        Task<AuthResponse> SignOutAsync(string id);

        Task<AuthResponse> RefreshToken(TokenModel tokens);
    }
}
