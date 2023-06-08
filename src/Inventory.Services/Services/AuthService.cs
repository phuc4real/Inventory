using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Helper;
using Inventory.Core.Options;
using Inventory.Core.Response;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JWTOption _option;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IOptionsSnapshot<JWTOption> option
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _option = option.Value;
        }

        public async Task<AuthResponse> ExternalLoginAsync()
        {
            AuthResponse response = new() { Messages = new List<ResponseMessage>() };
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if ( info != null )
            {
                var signinResult = await _signInManager.ExternalLoginSignInAsync(info!.LoginProvider, info.ProviderKey, false);
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var exist = await _userManager.FindByEmailAsync(email!);

                if(!signinResult.Succeeded)
                {
                    if (exist != null)
                    {
                        response.Status = ResponeStatus.STATUS_FAILURE;
                        response.Messages!.Add(new ResponseMessage() { Key = "User", Value = "Email already use!" });
                    }
                    else
                    {
                        AppUser newUser = new() {
                            UserName = Guid.NewGuid().ToString().Replace("-", ""),
                            Email = email,
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                    };

                        var res = await CreateUser(newUser, PasswordGenerator.Generate(16));

                        if (res.Status == ResponeStatus.STATUS_SUCCESS)
                        {
                            await _userManager.AddToRoleAsync(newUser, InventoryRoles.Employee);
                            await _userManager.AddLoginAsync(newUser, info);
                            response.Messages!.Add(new ResponseMessage() { Key = "User", Value = "User created successfully!" });
                        }
                    }
                }

                var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email)!);

                response.Token = await GetTokens(user!);
                response.Status = ResponeStatus.STATUS_SUCCESS;
            }
            else
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages!.Add(new ResponseMessage() { Key = "Errors", Value = "Something went wrong!" });
            }
            return response;
        }

        public async Task<AuthResponse> SignInAsync(string username, string password)
        {
            AuthResponse response = new() { Messages = new List<ResponseMessage>() };
            AppUser? user;

            if (IsEmail(username))
                user = await _userManager.FindByEmailAsync(username);
            else
                user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages!.Add(new ResponseMessage() { Key = "User", Value = "User not exists!" });
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

                if (result.Succeeded)
                {
                    response.Token = await GetTokens(user);
                    response.Messages!.Add(new ResponseMessage() { Key = "UserId", Value = user.Id });
                    response.Status = ResponeStatus.STATUS_SUCCESS;
                }
                else
                {
                    response.Status = ResponeStatus.STATUS_FAILURE;
                    response.Messages!.Add(new ResponseMessage() { Key = "User", Value = "Wrong password!" });
                }
            }
            return response;
        }

        public async Task<AuthResponse> SignUpAsync(string email, string username, string password)
        {
            AuthResponse response = new()
            {
                Messages = new List<ResponseMessage>()
            };

            var EmailExist = await _userManager.FindByEmailAsync(email) is not null;
            var UserNameExist = await _userManager.FindByNameAsync(username) is not null;
            if (EmailExist || UserNameExist)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages!.Add(new ResponseMessage() { Key = "User", Value = "User already exists!" });
            }
            else
            {
                AppUser user = new() { UserName = username, Email = email };
                var res = await CreateUser(user, password);
                if (res.Status == ResponeStatus.STATUS_SUCCESS)
                {
                    await _userManager.AddToRoleAsync(user, InventoryRoles.Employee);
                    response.Status = ResponeStatus.STATUS_SUCCESS;
                    response.Messages!.Add(new ResponseMessage() { Key = "User", Value = "User created successfully!" });
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

        public async Task<AuthResponse> SignOutAsync(string id)
        {
            AuthResponse response = new() { 
                Messages = new List<ResponseMessage>()
            };
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage() { Key = "User", Value = "User not exist!" });
            }
            else
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "Inventory", "RefreshToken");
                await _userManager.UpdateSecurityStampAsync(user);
                response.Status = ResponeStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage() { Key = "User", Value = "User logout!" });
            }

            return response;
        }

        public async Task<AuthResponse> RefreshToken(TokenModel tokens)
        {
            AuthResponse response = new() { Messages = new List<ResponseMessage>() };

            try
            {
                var principal = GetPrincipalFromExpiredToken(tokens.AccessToken!);
                
                if (principal == null)
                {
                    response.Status = ResponeStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage() { Key = "AccessToken", Value = "Token Invalid!" });
                }
                else
                {
                    var username = principal.Identity!.Name;
                    var user = await _userManager.FindByNameAsync(username!);

                    var storedToken = await _userManager.GetAuthenticationTokenAsync(user!, "Inventory", "RefreshToken");

                    var isValid = await _userManager.VerifyUserTokenAsync(user!, "Inventory", "RefreshToken", tokens.RefreshToken); 

                    if (isValid)
                    {
                        var newAccessToken = await GetTokens(user!);
                        response.Status = ResponeStatus.STATUS_SUCCESS;
                        response.Token = newAccessToken;
                    }
                    else
                    {
                        response.Status = ResponeStatus.STATUS_FAILURE;
                        response.Messages.Add(new ResponseMessage() { Key = "RefreshToken", Value = "Token Invalid!" });
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Status = ResponeStatus.STATUS_FAILURE;

                response.Messages.Add(new ResponseMessage() { Key = "AccessToken", Value = "Token Invalid!" });
                //response.Messages.Add(new ResponseMessage()
                //{
                //    Key = "SecurityTokenException",
                //    Value = ex.Message
                //});
                return response;
            }
        }

        private async Task<AuthResponse> CreateUser(AppUser user, string password)
        {
            AuthResponse response = new();
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                response.Status = ResponeStatus.STATUS_SUCCESS;                
            }
            else
            {
                response.Status = ResponeStatus.STATUS_FAILURE;
                response.Messages = result.Errors
                    .Select(x => new ResponseMessage() { Key = x.Code, Value = x.Description })
                    .ToList();
            }
            return response;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidateParameter = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _option.ValidIssuer,
                ValidateAudience = true,
                ValidAudience = _option.ValidAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_option.SecretKey)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidateParameter, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        private async Task<TokenModel> GetTokens(AppUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user, userRoles, 10);
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

        private SecurityToken GenerateToken(AppUser user, IList<string> userRoles, int expireMinutes)
        {
            var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_option.SecretKey));

            var token = new JwtSecurityToken(
                 audience: _option.ValidAudience,
                 issuer: _option.ValidIssuer,
                 expires: DateTime.Now.AddMinutes(expireMinutes),
                 claims: authClaims,
                 signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
                 );

            return token;
        }

        private static bool IsEmail(string email) => new EmailAddressAttribute().IsValid(email);
    }
}
