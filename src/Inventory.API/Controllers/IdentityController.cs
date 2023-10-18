using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using Inventory.Core.Enums;
using Inventory.Service;
using Inventory.Service.DTO.Identity;
using Inventory.Core.Common;
using Inventory.Service.Common;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _authService;

        public IdentityController(IIdentityService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.SignUpAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserIdentityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.SignInAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [Authorize]
        [HttpDelete("logout")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            BaseRequest request = new();
            request.SetContext(HttpContext);
            var result = await _authService.SignOutAsync(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("RefresshTokenLimit")]
        [ProducesResponseType(typeof(UserIdentityResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken()
        {
            BaseRequest request = new();
            request.SetContext(HttpContext);

            var refreshToken = HttpContext.GetRefreshToken();

            if (refreshToken.IsNullOrEmpty())
            {
                return BadRequest(new ResultMessage("RefreshToken", "Cannot get refresh token!"));
            }

            var result = await _authService.RefreshTokenAsync(request, refreshToken);

            return StatusCode((int)result.StatusCode, result);
        }

        //[AllowAnonymous]
        //[HttpGet("external-login")]
        //public IActionResult ExternalLogin(string? provider = "Google", string? returnUrl = "/")
        //{
        //    var properties = _authService.CreateAuthenticationProperties(provider!, returnUrl!);
        //    return new ChallengeResult(provider!, properties);
        //}

        //[AllowAnonymous]
        //[HttpGet("external-login-callback")]
        //[ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status409Conflict)]
        //public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var result = await _authService.ExternalLoginAsync();

        //    HttpContext.Response.Headers.Location = returnUrl;

        //    return result.Status == ResponseCode.Success ?
        //            Ok(result.Data) : StatusCode((int)result.Status, result.Message);

        //}
    }
}
