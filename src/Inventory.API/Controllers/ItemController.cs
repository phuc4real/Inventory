using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetList([FromQuery] string? name)
        {
            var queryString = Request.QueryString.ToString();
            if (_cacheService.TryGetCacheAsync(redisKey + ".List" + queryString, out List<ItemDetailDTO> response))
            {
                return Ok(response);
            }
            else
            {
                var result = await _itemService.GetList(name);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + ".List" + queryString, result.Data);
                    return Ok(result.Data);
                }

                //return StatusCode((int)result.Status,result.Message);
                return StatusCode((int)result.Status);
            }
        }

        [HttpGet]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(PaginationResponse<ItemDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest requestParams)
        {
            var queryString = Request.QueryString.ToString();
            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<ItemDetailDTO> response))
            {
                return Ok(response);
            }
            else
            {
                var result = await _itemService.GetPagination(requestParams);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + queryString, result);
                    return Ok(result);
                }

                return StatusCode((int)result.Status);
            }
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ItemDetailDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetItem(Guid id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id, out ItemDetailDTO items))
            {
                return Ok(items);
            }
            else
            {
                var result = await _itemService.GetById(id);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + "." + id, result.Data);
                    return Ok(result.Data);
                }

                return StatusCode((int)result.Status, result.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles =InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateItem(ItemEditDTO item)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.CreateItem(accessToken, item);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                Created("item/" + result.Data!.Id, result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateItem(Guid id, ItemEditDTO item)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.UpdateItem(accessToken, id, item);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var accessToken = await HttpContext.GetAccessToken();

            var result = await _itemService.DeleteItem(accessToken, id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : StatusCode((int)result.Status, result.Message);
        }
    }
}
