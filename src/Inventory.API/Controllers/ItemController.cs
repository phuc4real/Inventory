using Inventory.Core.Common;
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
        public async Task<IActionResult> ListItem()
        {
            var result = await _itemService.GetAll();

            return Ok(result.Data);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ItemDetailDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            var result = await _itemService.GetById(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result.Data);
            }
            else
            { 
                return NotFound(result.Messages); 
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ItemDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateItem(ItemEditDTO item)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var result = await _itemService.CreateItem(await GetToken(), item);

            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(Guid id, ItemEditDTO item)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var result = await _itemService.UpdateItem(await GetToken(), id, item);

            if(result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else { return NotFound(result.Messages);}
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var result = await _itemService.DeleteItem(await GetToken(), id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else { return NotFound(result.Messages); }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByName(string name)
        {
            var result = await _itemService.SearchByName(name);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else { return NotFound(result.Messages); }
        }

#pragma warning disable CS8603 // Possible null reference return.
        private async Task<string> GetToken() => await HttpContext.GetTokenAsync("access_token");
#pragma warning restore CS8603 // Possible null reference return.
    }
}
