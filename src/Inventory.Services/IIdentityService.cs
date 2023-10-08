using Inventory.Service.Common;
using Inventory.Service.DTO.Identity;

namespace Inventory.Service
{
    public interface IIdentityService
    {
        public Task<TokenObjectResponse> SignInAsync(LoginRequest request);

        public Task<BaseResponse> SignUpAsync(RegisterRequest request);

        public Task<BaseResponse> SignOutAsync(string token);

        public Task<TokenObjectResponse> RefreshTokenAsync(string accessToken, string refreshToken);

        //Task<ResultResponse<TokenModel>> ExternalLoginAsync();

        //AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl);
    }
}
