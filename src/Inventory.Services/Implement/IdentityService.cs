using Azure.Core;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Model.Entity;
using Inventory.Service.Common;
using Inventory.Service.DTO.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;

namespace Inventory.Service.Implement
{
    public class IdentityService : IIdentityService
    {
        #region Ctor & Field

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        private readonly string provider = "Inventory Identity";
        private readonly string tokenName = "Refresh Token";

        public IdentityService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        #endregion

        #region Method

        public async Task<IdentityObjectResponse> SignInAsync(LoginRequest request)
        {
            IdentityObjectResponse response = new();

            //Check if user is input username or email and get user
            var user = IsEmail(request.Username) ?
                await _userManager.FindByEmailAsync(request.Username) :
                await _userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("UErrorser", "User not exists!");
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

                if (result.Succeeded)
                {
                    var tokens = await GetIdentityResponseAsync(user);

                    var refreshTokenExpireTime = DateTime.UtcNow.AddDays(15);

                    user.RefreshTokenExpireTime = refreshTokenExpireTime;

                    await _userManager.UpdateAsync(user);

                    response.Data = tokens;
                }
                else
                {
                    response.StatusCode = ResponseCode.BadRequest;
                    response.Message = new("Error", "Wrong password!");
                }
            }
            return response;
        }

        public async Task<BaseResponse> SignUpAsync(RegisterRequest request)
        {
            BaseResponse response = new();

            bool emailExist = await _userManager.FindByEmailAsync(request.Email) is not null;
            bool userNameExist = await _userManager.FindByNameAsync(request.Username) is not null;

            if (emailExist || userNameExist)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Error", "User already exists!");
            }
            else
            {
                AppUser user = new()
                {
                    UserName = request.Username,
                    Email = request.Email
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, InventoryRoles.NormalUser);
                    response.Message = new("Success", "User created successfully!");
                }
                else
                {
                    response.StatusCode = ResponseCode.BadRequest;
                    response.Message = new("Error", "User info invalid!");
                }
            }

            return response;
        }

        public async Task<BaseResponse> SignOutAsync(BaseRequest request)
        {
            BaseResponse response = new();
            var user = await _userManager.FindByIdAsync(request.GetUserContext());

            if (user == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Error", "User not exist!");
            }
            else
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, provider, tokenName);

                user.RefreshTokenExpireTime = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);

                response.Message = new("Success", "User logout!");
            }

            return response;
        }

        public async Task<IdentityObjectResponse> RefreshTokenAsync(BaseRequest request)
        {
            IdentityObjectResponse response = new();

            var user = await _userManager.FindByIdAsync(request.GetUserContext());

            var (accesToken, refreshToken) = request.GetFullToken();

            var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, provider, tokenName);
            var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(user, provider, "rs-" + user.Id, refreshToken);
            var curDateTime = DateTime.UtcNow;

            bool isValid = isRefreshTokenValid
                         && curDateTime < user.RefreshTokenExpireTime
                         && storedRefreshToken == refreshToken;

            if (isValid)
            {
                response.Data = await GetIdentityResponseAsync(user);
            }
            else
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Error", "Refresh token Invalid!");
            }

            return response;
        }

        //public AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl)
        //{
        //    var url = $"https://localhost:5001/api/auth/external-login-callback?returnUrl={returnUrl}";
        //    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, url);
        //    properties.AllowRefresh = true;
        //    return properties;
        //}

        //public async Task<ResultResponse<TokenModel>> ExternalLoginAsync()
        //{
        //    ResultResponse<TokenModel> response = new();

        //    var info = await _signInManager.GetExternalLoginInfoAsync();

        //    if (info == null)
        //    {
        //        response.StatusCode = ResponseCode.BadRequest;
        //        response.Message = new("Error", "Something went wrong!");
        //    }
        //    else
        //    {
        //        var signinResult = await _signInManager.ExternalLoginSignInAsync(info!.LoginProvider, info.ProviderKey, false);
        //        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        //        var user = await _userManager.FindByEmailAsync(email!);

        //        if (!signinResult.Succeeded)
        //        {
        //            if (user != null)
        //            {
        //                response.StatusCode = ResponseCode.Conflict;
        //                response.Message = new("User", "Email already use!");
        //            }
        //            else
        //            {
        //                AppUserEntity newUser = new()
        //                {
        //                    UserName = Guid.NewGuid().ToString().Replace("-", ""),
        //                    Email = email,
        //                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
        //                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
        //                };

        //                var res = await CreateUser(newUser, StringHelper.PasswordGenerate(16));

        //                if (res)
        //                {
        //                    response.StatusCode = ResponseCode.Success;
        //                    await _userManager.AdrequestRoleAsync(newUser, InventoryRoles.NormalUser);
        //                    await _userManager.AddLoginAsync(newUser, info);
        //                    response.Message = new("User", "User created successfully!");
        //                }
        //                else
        //                {
        //                    response.StatusCode = ResponseCode.BadRequest;
        //                    response.Message = new("User", "User info invalid!");
        //                }

        //                return response;
        //            }
        //        }

        //        response.Data = await GetTokens(user!);
        //        response.StatusCode = ResponseCode.Success;
        //    }
        //    return response;
        //}

        #endregion

        #region Private

        private async Task<UserIdentityResponse> GetIdentityResponseAsync(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateToken(user, userRoles.ToList());

            var refreshToken = await _userManager.GenerateUserTokenAsync(user, provider, "rs-" + user.Id);

            await _userManager.SetAuthenticationTokenAsync(user, provider, tokenName, refreshToken);

            UserIdentityResponse res = new()
            {
                UserId = user.Id,
                UserName = user.UserName,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                ExpireTime = token.ValidTo
            };

            return res;
        }

        private static bool IsEmail(string email) => new EmailAddressAttribute().IsValid(email);


        #endregion

    }
}
