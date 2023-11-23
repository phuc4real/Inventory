using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Identity;

namespace Inventory.Service
{
    public interface IIdentityService
    {
        public Task<IdentityObjectResponse> SignInAsync(LoginRequest request);

        public Task<BaseResponse> SignUpAsync(RegisterRequest request);

        public Task<BaseResponse> SignOutAsync(BaseRequest request);

        public Task<IdentityObjectResponse> RefreshTokenAsync(BaseRequest request);

        //Task<ResultResponse<TokenModel>> ExternalLoginAsync();

        //AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl);
    }
}
