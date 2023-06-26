using Inventory.Core.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.RateLimiting;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SignUpAsync(dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : BadRequest(result.Messages);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SignInAsync(dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : BadRequest(result.Messages);
        }

        //Change to HttpPost if have front-end
        [AllowAnonymous]
        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string? provider = "Google", string? returnUrl = "/")
        {
            var properties = _authService.CreateAuthenticationProperties(provider!, returnUrl!);
            return new ChallengeResult(provider!, properties);
        }

        //Change to HttpPost if have front-end
        [AllowAnonymous]
        [HttpGet("external-login-callback")]
        [ProducesResponseType(typeof(ResultResponse<TokenModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var result = await _authService.ExternalLoginAsync();

            HttpContext.Response.Headers.Location = returnUrl;
            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : BadRequest(result.Messages);

        }

        [HttpDelete("logout")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            string token = await HttpContext.GetAccessToken();

            var result = await _authService.SignOutAsync(token);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : BadRequest(result.Messages);
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("RefresshTokenLimit")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken()
        {
            var accessToken = await HttpContext.GetAccessToken();
            var refreshToken = HttpContext.GetRefreshToken();

            if (refreshToken == "")
            {
                return BadRequest(new ResponseMessage("RefreshToken","Cannot get refresh token!"));
            }

            var result = await _authService.RefreshToken(accessToken, refreshToken);

            return result.Status == ResponseStatus.STATUS_SUCCESS ? 
                    Ok(result.Data) : BadRequest(result.Messages);
        }


        [HttpPost("grant-role")]
        [Authorize(Roles =InventoryRoles.Admin)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GrantPermission(GrantRoleDTO dto)
        {
            var result = await _authService.GrantPermission(dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                     Ok(result.Messages) : BadRequest(result.Messages);
        }
    }
}
