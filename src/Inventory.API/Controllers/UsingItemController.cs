using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsingItemController : ControllerBase
    {
        private readonly IUsingItemService _usingItemService;

        public UsingItemController(IUsingItemService usingItemService)
        {
            _usingItemService = usingItemService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsingItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListUsingItem()
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _usingItemService.GetUsingItemByRole(token);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result.Data);
            }

            return NotFound(result.Messages);
        }

        [HttpGet("my-list")]
        [ProducesResponseType(typeof(IEnumerable<UsingItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MyUsingItem()
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _usingItemService.MyUsingItem(token);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result.Data);
            }

            return NotFound(result.Messages);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<UsingItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchUsingItem(string filter)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _usingItemService.SearchForUsingItem(token, filter);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result.Data);
            }

            return NotFound(result.Messages);
        }
    }
}
