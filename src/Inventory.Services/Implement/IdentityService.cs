using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.ViewModel;
using Inventory.Model.Entity;
using Inventory.Service.Common;
using Inventory.Service.DTO.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Inventory.Service.Implement
{
    public class IdentityService : IIdentityService
    {
        #region Ctor & Field

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

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

        public async Task<TokenObjectResponse> SignInAsync(LoginRequest request)
        {
            TokenObjectResponse response = new();

            //Check if user is input username or email and get user
            var user = IsEmail(request.Username) ?
                await _userManager.FindByEmailAsync(request.Username) :
                await _userManager.FindByNameAsync(request.Username);


            if (user == null)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("User", "User not exists!");
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

                if (result.Succeeded)
                {
                    var tokens = await GetJwtToken(user);

                    var refreshTokenExpireTime = DateTime.UtcNow.AddMinutes(60);

                    user.RefreshTokenExpireTime = refreshTokenExpireTime;

                    await _userManager.UpdateAsync(user);

                    response.Data = tokens;
                    response.StatusCode = ResponseCode.Success;
                }
                else
                {
                    response.StatusCode = ResponseCode.BadRequest;
                    response.Message = new("User", "Wrong password!");
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
                response.Message = new("User", "User already exists!");
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
                    response.StatusCode = ResponseCode.Success;
                    response.Message = new("User", "User created successfully!");
                }
                else
                {
                    response.StatusCode = ResponseCode.BadRequest;
                    response.Message = new("User", "User info invalid!");
                }
            }

            return response;
        }

        public async Task<BaseResponse> SignOutAsync(string token)
        {
            BaseResponse response = new();

            var user = await _userManager.FindByIdAsync(_tokenService.GetUserId(token));

            if (user == null)
            {
                response.StatusCode = ResponseCode.NotFound;
                response.Message = new("User", "User not exist!");
            }
            else
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "Inventory Indentity", "Refresh Token");

                user.RefreshTokenExpireTime = DateTime.UtcNow;

                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);

                response.StatusCode = ResponseCode.Success;
                response.Message = new("User", "User logout!");
            }

            return response;
        }

        public async Task<TokenObjectResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            TokenObjectResponse response = new();

            try
            {
                var userId = _tokenService.GetUserId(accessToken);

                if (string.IsNullOrEmpty(userId))
                {
                    response.StatusCode = ResponseCode.BadRequest;
                    response.Message = new("AccessToken", "Access Token Invalid!");
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(userId!);

                    var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "Inventory Indentity", "Refresh Token");
                    var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(user, "Inventory Indentity", "rs-" + user.Id, refreshToken);
                    var curDateTime = DateTime.UtcNow;

                    bool isValid = isRefreshTokenValid
                                   && curDateTime < user.RefreshTokenExpireTime
                                   && storedRefreshToken == refreshToken;

                    if (isValid)
                    {
                        var newToken = await GetJwtToken(user);
                        response.StatusCode = ResponseCode.Success;
                        response.Data = newToken;
                    }
                    else
                    {
                        response.StatusCode = ResponseCode.BadRequest;
                        response.Message = new("RefreshToken", "Refresh token Invalid!");
                    }
                }

                return response;
            }
            catch (Exception)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("AccessToken", "Access token Invalid!");

                return response;
            }
        }

        private async Task<TokenResponse> GetJwtToken(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.GenerateToken(user, userRoles.ToList());

            var refreshToken = await _userManager.GenerateUserTokenAsync(user, "Inventory Indentity", "rs-" + user.Id);

            await _userManager.SetAuthenticationTokenAsync(user, "Inventory Indentity", "Refresh Token", refreshToken);

            TokenResponse res = new()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                ExpireTime = token.ValidTo
            };

            return res;
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

        private static bool IsEmail(string email) => new EmailAddressAttribute().IsValid(email);


        #endregion

    }
}
