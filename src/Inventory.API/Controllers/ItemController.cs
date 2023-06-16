using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ItemDetailDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListItem()
        {
            var result = await _itemService.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("in-use")]
        [ProducesResponseType(typeof(IEnumerable<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItemInUse()
        {
            var result = await _itemService.GetItemInUse();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ItemDetailDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var result = await _itemService.GetById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateItem(ItemEditDTO item)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.CreateItem(accessToken, item);

            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(Guid id, ItemEditDTO item)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.UpdateItem(accessToken, id, item);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.DeleteItem(accessToken, id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByName(string name)
        {
            var result = await _itemService.SearchByName(name);

            return result.Status == ResponseStatus.STATUS_SUCCESS ? 
                    Ok(result) : NotFound(result.Messages);
        }
    }
}
