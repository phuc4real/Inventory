using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Inventory.Services.Services;
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
        [ProducesResponseType(typeof(List<AppUser>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetList([FromQuery] string? filter)
        {
            var result = await _userService.GetList(filter);

            return result.Status == ResponseCode.Success ?
                Ok(result.Data) : NoContent();
        }

        [HttpGet("info/{id}")]
        [ProducesResponseType(typeof(AppUserDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> InfoOfId(string id)
        {
            var result = await _userService.GetById(id);

            return result.Status == ResponseCode.Success ?
                Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("info")]
        [ProducesResponseType(typeof(AppUserDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UserInfo()
        {
            var result = await _userService.GetByToken(await HttpContext.GetAccessToken());

            return result.Status == ResponseCode.Success ?
                Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }
    }
}
