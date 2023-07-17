﻿using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
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
using System.Text;

namespace Inventory.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
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

            //TODO: Kiểm tra ==null trước để tra ra lỗi ngay lập tức.
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
                        AppUser newUser = new()
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
                            await _userManager.AddToRoleAsync(newUser, InventoryRoles.Employee);
                            await _userManager.AddLoginAsync(newUser, info);
                            response.Message = new("User", "User created successfully!");
                        }
                        else
                        {
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("User", "User info invalid!");
                        }
                        //TODO: Chổ này có cần có else khum? nếu khum create user thành công thì return response luôn k làm cái dưới.

                        return response;
                    }
                }
                //TODO: FindByEmailAsync có email đã khởi tạo ở trên rồi, e nên dùng lại.

                response.Data = await GetTokens(user!);
                response.Status = ResponseCode.Success;
            }
            return response;
        }

        public async Task<ResultResponse<TokenModel>> SignInAsync(LoginDTO dto)
        {
            ResultResponse<TokenModel> response = new();

            //TOD0: sử dụng "toán tử điều kiện" cho ngắn gọn hơn vì trong if else cùng làm 1 công việc.

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

        public async Task<ResultResponse<TokenModel>> SignUpAsync(RegisterDTO dto)
        {
            ResultResponse<TokenModel> response = new();
            //TODO: following the C# naming conventions(EmailExist, UserNameExist).
            var emailExist = await _userManager.FindByEmailAsync(dto.Email!) is not null;
            var userNameExist = await _userManager.FindByNameAsync(dto.Username!) is not null;

            if (emailExist || userNameExist)
            {
                response.Status = ResponseCode.Conflict;
                response.Message = new("User", "User already exists!");
            }
            else
            {
                AppUser user = new() 
                { 
                    UserName = dto.Username, 
                    Email = dto.Email 
                };

                var res = await CreateUser(user, dto.Password!);

                if (res)
                {
                    await _userManager.AddToRoleAsync(user, InventoryRoles.Employee);
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

        public async Task<ResultResponse<TokenModel>> SignOutAsync(string token)
        {
            ResultResponse<TokenModel> response = new();

            var userId = _tokenService.GetUserId(token);
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

                    //TODO: đặt tên cho dễ hiểu hơn (storedRefreshToken,isRefreshTokenValid) đặt tên là vấn nạn với a :V
                    var storedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user!, "Inventory", "RefreshToken");

                    var isRefreshTokenValid = await _userManager.VerifyUserTokenAsync(user!, "Inventory", "RefreshToken", refreshToken);

                    var curDateTime = DateTime.UtcNow;

                    //TODO: isValid = isValidRefreshToken 
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

        private async Task<bool> CreateUser(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        private async Task<TokenModel> GetTokens(AppUser user)
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

        public async Task<ResultResponse<AppUserDTO>> GrantPermission(GrantRoleDTO dto)
        {
            ResultResponse<AppUserDTO> response = new();

            var user = await _userManager.FindByIdAsync(dto.UserId!);
            if (user == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("User", "User not found!");
            }
            else
            {
                var role = dto.Role.ToDescriptionString();
                var hasRole = await _userManager.IsInRoleAsync(user,role!);
                if (hasRole)
                {
                    response.Status = ResponseCode.Conflict;
                    response.Message = new("User", $"User already has role {role}!");
                }
                else
                {
                    response.Status = ResponseCode.Success;
                    await _userManager.AddToRoleAsync(user, role!);
                    response.Message = new("User", $"Grant role {role} to {user.UserName}");
                }
            }
            return response;
        }
    }
}
