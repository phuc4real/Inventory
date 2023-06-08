using Inventory.API.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.SignUpAsync(dto.Email, dto.Username,dto.Password);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Messages);
            else 
                return BadRequest(result.Messages);
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.SignInAsync(dto.Username!, dto.Password!);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Token);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await _authService.ExternalLoginAsync();

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpDelete("logout/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken(TokenModel tokens)
        {
            var result = await _authService.RefreshToken(tokens);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Messages);
            }
        }
    }
}
