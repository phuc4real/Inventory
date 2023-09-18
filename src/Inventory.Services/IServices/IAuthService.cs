using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Microsoft.AspNetCore.Authentication;

namespace Inventory.Services.IServices
{
    public interface IAuthService
    {
        Task<ResultResponse<TokenModel>> SignInAsync(Login dto);
        Task<ResultResponse> SignUpAsync(Register dto);
        //Task<ResultResponse<TokenModel>> ExternalLoginAsync();
        Task<ResultResponse> SignOutAsync(string token);
        Task<ResultResponse<TokenModel>> RefreshToken(string accessToken, string refreshToken);
        //AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl);
    }
}
