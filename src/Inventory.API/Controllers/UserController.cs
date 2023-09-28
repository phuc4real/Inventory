using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Service;
using Inventory.Service.Common;
using Inventory.Service.DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(UserListResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList([FromQuery] string? search)
        {
            var result = await _userService.GetListAsync(search);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("info/{id}")]
        [ProducesResponseType(typeof(UserObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InfoOfId(string id)
        {
            var result = await _userService.GetByIdAsync(id);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("info")]
        [ProducesResponseType(typeof(UserObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserInfo()
        {
            var result = await _userService.GetAsync(await HttpContext.GetAccessToken());

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
