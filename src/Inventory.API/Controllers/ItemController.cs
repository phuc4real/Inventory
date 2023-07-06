using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Core;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "Inventory.Item";

        public ItemController(IItemService itemService, IRedisCacheService cacheService)
        {
            _itemService = itemService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListItem([FromQuery] PaginationRequest requestParams)
        {
            var queryString = Request.QueryString.ToString();
            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<ItemDetailDTO> response))
            {
                return Ok(response);
            }
            else
            {
                var result = await _itemService.GetAll(requestParams);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + queryString, result);
                    return Ok(result);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ItemDetailDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id, out ItemDetailDTO items))
            {
                return Ok(items);
            }
            else
            {
                var result = await _itemService.GetById(id);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + "." + id, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpPost]
        [Authorize(Roles =InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateItem(ItemEditDTO item)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.CreateItem(accessToken, item);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Created("item/" + result.Data!.Id, result.Messages) : NotFound(result.Messages);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResultResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(Guid id, ItemEditDTO item)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.UpdateItem(accessToken, id, item);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.DeleteItem(accessToken, id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : NotFound(result.Messages);
        }
    }
}
