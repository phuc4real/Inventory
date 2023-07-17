using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
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
    }
}
