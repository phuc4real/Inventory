using Inventory.Core.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.SignUpAsync(dto);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Messages);
            else 
                return BadRequest(result.Messages);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultResponse<TokenModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.SignInAsync(dto);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result);
            else
                return BadRequest(result.Messages);
        }

        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string? provider = "Google", string? returnUrl = "/home")
        {
            var properties = _authService.CreateAuthenticationProperties(provider!, returnUrl!);
            return new ChallengeResult(provider!, properties);
        }

        [HttpGet("external-login-callback")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultResponse<TokenModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await _authService.ExternalLoginAsync();

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result);
            else
                return BadRequest(result.Messages);
        }

        [HttpDelete("logout/{id}")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout(string id)
        {
            var result = await _authService.SignOutAsync(id);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
            {
                return Ok(result.Messages);
            }
            else
            {
                return BadRequest(result.Messages);
            }
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken(TokenModel tokens)
        {
            var result = await _authService.RefreshToken(tokens);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.Messages);
            }
        }
    }
}
