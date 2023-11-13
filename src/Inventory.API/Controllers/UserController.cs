using Inventory.Core.Common;
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

        [HttpGet()]
        [ProducesResponseType(typeof(UserPaginationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(PaginationRequest request)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.GetListAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("info/{userName}")]
        [ProducesResponseType(typeof(UserObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetInfoByUserName(string userName)
        {
            var result = await _userService.GetByUserNameAsync(userName);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("info")]
        [ProducesResponseType(typeof(UserObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserInfo()
        {
            var request = new BaseRequest();
            request.SetContext(HttpContext);
            var result = await _userService.GetAsync(request);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
