using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> GetItemInUse()
        {
            var result = await _usingItemService.GetAllUsingItemAsync();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<UsingItemDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchUsingItem(string filter)
        {
            var result = await _usingItemService.SearchForUsingItemAsync(filter);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }
    }
}
