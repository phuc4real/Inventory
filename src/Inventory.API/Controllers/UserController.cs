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

        [HttpGet]
        [Authorize(Roles = InventoryRoles.SuperAdmin)]
        [ProducesResponseType(typeof(PaginationResponse<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPagination([FromQuery]PaginationRequest request)
        {
            var result = await _userService.GetPagination(request);

            return result.Status == ResponseCode.Success ?
                Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetList()
        {
            var result = await _userService.GetList();

            return result.Status == ResponseCode.Success?
                Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _userService.GetById(id);

            return result.Status == ResponseCode.Success ?
                Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }

        [HttpPost("grant-role")]
        [Authorize(Roles = InventoryRoles.SuperAdmin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> GrantPermission(GrantRoleDTO dto)
        {
            var result = await _userService.GrantPermission(dto);

            return result.Status == ResponseCode.Success ?
                     Ok(result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("{id}/check-role")]
        [ProducesResponseType(typeof(RoleDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckRole(string id)
        {
            var result = await _userService.CheckRole(id);

            return Ok(result.Data);
        }


        [HttpGet("info")]
        [ProducesResponseType(typeof(AppUserDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> UserInfo()
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _userService.GetUserInfo(token);

            return Ok(result.Data);
        }
    }
}
