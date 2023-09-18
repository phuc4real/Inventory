using Inventory.Core.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.RateLimiting;
using Inventory.Core.Enums;

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
        public async Task<IActionResult> Register(Register dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SignUpAsync(dto);

            return StatusCode((int)result.Status, result.Message);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login(Login dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.SignInAsync(dto);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Data) : StatusCode((int)result.Status, result.Message);
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

        [HttpDelete("logout")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            string token = await HttpContext.GetAccessToken();

            var result = await _authService.SignOutAsync(token);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [AllowAnonymous]
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
                return BadRequest(new ResponseMessage("RefreshToken", "Cannot get refresh token!"));
            }

            var result = await _authService.RefreshToken(accessToken, refreshToken);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }
    }
}
