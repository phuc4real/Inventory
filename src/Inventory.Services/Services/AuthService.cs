using Inventory.Core.Common;
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

            ResultResponse<TokenModel> response = new()
            {
                Messages = new List<ResponseMessage>()
            };
            var info = await _signInManager.GetExternalLoginInfoAsync();
            //TODO: Kiểm tra ==null trước để tra ra lỗi ngay lập tức.
            if (info != null)
            {
                var signinResult = await _signInManager.ExternalLoginSignInAsync(info!.LoginProvider, info.ProviderKey, false);
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var exist = await _userManager.FindByEmailAsync(email!);

                if (!signinResult.Succeeded)
                {
                    if (exist != null)
                    {
                        response.Status = ResponseStatus.STATUS_FAILURE;
                        response.Messages!.Add(new ResponseMessage("User", "Email already use!"));
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

                        var res = await CreateUser(newUser, PasswordGenerator.Generate(16));

                        if (res.Status == ResponseStatus.STATUS_SUCCESS)
                        {
                            await _userManager.AddToRoleAsync(newUser, InventoryRoles.Employee);
                            await _userManager.AddLoginAsync(newUser, info);
                            response.Messages!.Add(new ResponseMessage("User", "User created successfully!"));
                        }
                        //TODO: Chổ này có cần có else khum? nếu khum create user thành công thì return response luôn k làm cái dưới.
                    }
                }
                //TODO: FindByEmailAsync có email đã khởi tạo ở trên rồi, e nên dùng lại.
                var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email)!);

                response.Data = await GetTokens(user!);
                response.Status = ResponseStatus.STATUS_SUCCESS;
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages!.Add(new ResponseMessage("Error", "Something went wrong!"));
            }
            return response;
        }

        public async Task<ResultResponse<TokenModel>> SignInAsync(LoginDTO dto)
        {
            ResultResponse<TokenModel> response = new() { Messages = new List<ResponseMessage>() };

            //TOD0: sử dụng "toán tử điều kiện" cho ngắn gọn hơn vì trong if else cùng làm 1 công việc.
            AppUser? user;

            if (IsEmail(dto.Username!))
            {
                user = await _userManager.FindByEmailAsync(dto.Username!);
            }
            else
            {
                user = await _userManager.FindByNameAsync(dto.Username!);
            }

            if (user == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages!.Add(new ResponseMessage("User", "User not exists!"));
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, false);

                if (result.Succeeded)
                {
                    var tokens = await GetTokens(user);
                    var refreshTokenExpireTime = DateTime.UtcNow.AddMinutes(30);
                    user.RefreshTokenExpireTime = refreshTokenExpireTime;
                    await _userManager.UpdateAsync(user);

                    response.Data = tokens;
                    response.Status = ResponseStatus.STATUS_SUCCESS;
                }
                else
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages!.Add(new ResponseMessage("User", "Wrong password!"));
                }
            }
            return response;
        }

        public async Task<ResultResponse<TokenModel>> SignUpAsync(RegisterDTO dto)
        {
            ResultResponse<TokenModel> response = new()
            {
                Messages = new List<ResponseMessage>()
            };
            //TODO: following the C# naming conventions(EmailExist, UserNameExist).
            var EmailExist = await _userManager.FindByEmailAsync(dto.Email!) is not null;
            var UserNameExist = await _userManager.FindByNameAsync(dto.Username!) is not null;
            if (EmailExist || UserNameExist)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages!.Add(new ResponseMessage("User", "User already exists!"));
            }
            else
            {
                AppUser user = new() { UserName = dto.Username, Email = dto.Email };
                var res = await CreateUser(user, dto.Password!);
                if (res.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _userManager.AddToRoleAsync(user, InventoryRoles.Employee);
                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    response.Messages!.Add(new ResponseMessage("User", "User created successfully!"));
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
            ResultResponse<TokenModel> response = new()
            {
                Messages = new List<ResponseMessage>()
            };

            //TODO: following the C# naming conventions(userId)
            var userid = _tokenService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userid);

            if (user == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("User", "User not exist!"));
            }
            else
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "Inventory", "RefreshToken");
                user.RefreshTokenExpireTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("User", "User logout!"));
            }

            return response;
        }

        public async Task<ResultResponse<TokenModel>> RefreshToken(string accessToken, string refreshToken)
        {
            ResultResponse<TokenModel> response = new() { Messages = new List<ResponseMessage>() };

            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

                if (principal == null)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("AccessToken", "Token Invalid!"));
                }
                else
                {
                    //var username = principal.Identity!.Name;

                    //TODO: following the C# naming conventions(userId)
                    var userid = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _userManager.FindByIdAsync(userid!);

                    //TODO: đặt tên cho dễ hiểu hơn (storedRefreshToken,isRefreshTokenValid) đặt tên là vấn nạn với a :V
                    var storedToken = await _userManager.GetAuthenticationTokenAsync(user!, "Inventory", "RefreshToken");

                    var refreshTokenValid = await _userManager.VerifyUserTokenAsync(user!, "Inventory", "RefreshToken", refreshToken);

                    var curDateTime = DateTime.UtcNow;

                    //TODO: isValid = isValidRefreshToken 
                    bool isValid = refreshTokenValid
                                   && curDateTime < user!.RefreshTokenExpireTime
                                   && storedToken == refreshToken;

                    if (isValid)
                    {
                        var newAccessToken = await GetTokens(user!);
                        response.Status = ResponseStatus.STATUS_SUCCESS;
                        response.Data = newAccessToken;
                    }
                    else
                    {
                        response.Status = ResponseStatus.STATUS_FAILURE;
                        response.Messages.Add(new ResponseMessage("RefreshToken", "Token Invalid!"));
                    }
                }

                return response;
            }
            catch (Exception)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("AccessToken", "Token Invalid!"));
                return response;
            }
        }

        private async Task<ResultResponse<TokenModel>> CreateUser(AppUser user, string password)
        {
            ResultResponse<TokenModel> response = new();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages = result.Errors
                    .Select(x => new ResponseMessage(x.Code, x.Description))
                    .ToList();
            }
            return response;
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
            ResultResponse<AppUserDTO> response = new()
            {
                Messages = new List<ResponseMessage>()
            };

            var user = await _userManager.FindByIdAsync(dto.UserId!);
            if (user == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("User", "User not found!"));
            }
            else
            {
                var role = dto.Role.ToDescriptionString();
                var hasRole = await _userManager.IsInRoleAsync(user,role!);
                if (!hasRole)
                {
                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    await _userManager.AddToRoleAsync(user, role!);
                    response.Messages.Add(new ResponseMessage("User", $"Grant role {role} to {user.UserName}"));
                }
                else
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("User", $"User already has role {role}!"));
                }

            }

            return response;
        }
    }
}
