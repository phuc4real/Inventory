using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Helper;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Inventory.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly SignInManager<AppUserEntity> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUserEntity> userManager,
            SignInManager<AppUserEntity> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<TokenModel>> ExternalLoginAsync()
        {
            ResultResponse<TokenModel> response = new();

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                response.Status = ResponseCode.BadRequest;
                response.Message = new("Error", "Something went wrong!");
            }
            else
            {
                var signinResult = await _signInManager.ExternalLoginSignInAsync(info!.LoginProvider, info.ProviderKey, false);
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = await _userManager.FindByEmailAsync(email!);

                if (!signinResult.Succeeded)
                {
                    if (user != null)
                    {
                        response.Status = ResponseCode.Conflict;
                        response.Message = new("User", "Email already use!");
                    }
                    else
                    {
                        AppUserEntity newUser = new()
                        {
                            UserName = Guid.NewGuid().ToString().Replace("-", ""),
                            Email = email,
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                        };

                        var res = await CreateUser(newUser, StringHelper.PasswordGenerate(16));

                        if (res)
                        {
                            response.Status = ResponseCode.Success;
                            await _userManager.AddToRoleAsync(newUser, InventoryRoles.NormalUser);
                            await _userManager.AddLoginAsync(newUser, info);
                            response.Message = new("User", "User created successfully!");
                        }
                        else
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("User", "User info invalid!");
                        }

                        return response;
                    }
                }

                response.Data = await GetTokens(user!);
                response.Status = ResponseCode.Success;
            }
            return response;
        }

        public async Task<ResultResponse<TokenModel>> SignInAsync(Login dto)
        {
            ResultResponse<TokenModel> response = new();

            var user = IsEmail(dto.Username!) ?
                await _userManager.FindByEmailAsync(dto.Username!) :
                await _userManager.FindByNameAsync(dto.Username!);

            if (user == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("User", "User not exists!");
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

                if (result.Succeeded)
                {
                    var tokens = await GetTokens(user);
                    var refreshTokenExpireTime = DateTime.UtcNow.AddMinutes(60);
                    user.RefreshTokenExpireTime = refreshTokenExpireTime;
                    await _userManager.UpdateAsync(user);

                    response.Data = tokens;
                    response.Status = ResponseCode.Success;
                }
                else
                {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("User", "Wrong password!");
                }
            }
            return response;
        }

        public async Task<ResultResponse> SignUpAsync(Register dto)
        {
            ResultResponse response = new();

            var emailExist = await _userManager.FindByEmailAsync(dto.Email!) is not null;
            var userNameExist = await _userManager.FindByNameAsync(dto.Username!) is not null;

            if (emailExist || userNameExist)
            {
                response.Status = ResponseCode.Conflict;
                response.Message = new("User", "User already exists!");
            }
            else
            {
                AppUserEntity user = new()
                {
                    UserName = dto.Username,
                    Email = dto.Email
                };

                var res = await CreateUser(user, dto.Password!);

                if (res)
                {
                    await _userManager.AddToRoleAsync(user, InventoryRoles.NormalUser);
                    response.Status = ResponseCode.Success;
                    response.Message = new("User", "User created successfully!");
                }
                else
                {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("User", "User info invalid!");
                }
            }

            return response;
        }

        public AuthenticationProperties CreateAuthenticationProperties(string provider, string returnUrl)
        {
            var url = $"https://localhost:5001/api/auth/external-login-callback?returnUrl={returnUrl}";
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, url);
            properties.AllowRefresh = true;
            return properties;
        }

        public async Task<ResultResponse> SignOutAsync(string token)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("User", "User not exist!");
            }
            else
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "Inventory", "RefreshToken");
                user.RefreshTokenExpireTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);

                response.Status = ResponseCode.Success;
                response.Message = new("User", "User logout!");
            }

            return response;
        }

        public async Task<ResultResponse<TokenModel>> RefreshToken(string accessToken, string refreshToken)
        {
            ResultResponse<TokenModel> response = new();

            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

                if (principal == null)
                {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("AccessToken", "Access Token Invalid!");
                }
                else
                {
                    var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userId!);

                    var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user!, "Inventory", "RefreshToken");
                    var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(user!, "Inventory", "RefreshToken", refreshToken);
                    var curDateTime = DateTime.UtcNow;

                    bool isValid = isRefreshTokenValid
                                   && curDateTime < user!.RefreshTokenExpireTime
                                   && storedRefreshToken == refreshToken;

                    if (isValid)
                    {
                        var newToken = await GetTokens(user!);
                        response.Status = ResponseCode.Success;
                        response.Data = newToken;
                    }
                    else
                    {
                        response.Status = ResponseCode.BadRequest;
                        response.Message = new("RefreshToken", "Refresh token Invalid!");
                    }
                }

                return response;
            }
            catch (Exception)
            {
                response.Status = ResponseCode.BadRequest;
                response.Message = new("AccessToken", "Access token Invalid!");

                return response;
            }
        }

        private async Task<bool> CreateUser(AppUserEntity user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        private async Task<TokenModel> GetTokens(AppUserEntity user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, userRoles);
            var refreshToken = await _userManager.GenerateUserTokenAsync(user, "Inventory", "RefreshToken");
            await _userManager.SetAuthenticationTokenAsync(user, "Inventory", "RefreshToken", refreshToken);

            TokenModel res = new()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                ExpireTime = token.ValidTo
            };

            return res;
        }

        private static bool IsEmail(string email) => new EmailAddressAttribute().IsValid(email);
    }
}
