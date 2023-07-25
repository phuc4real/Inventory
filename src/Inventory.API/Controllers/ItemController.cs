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
        [ProducesResponseType(typeof(List<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> List([FromQuery] string? name)
        {
            var queryString = Request.QueryString.ToString();
            if (_cacheService.TryGetCacheAsync(redisKey + ".List" + queryString, out List<Item> response))
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

                return StatusCode((int)result.Status);
            }
        }

        [HttpGet]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(PaginationResponse<Item>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest requestParams)
        {
            var queryString = Request.QueryString.ToString();
            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<Item> response))
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
        [ProducesResponseType(typeof(ItemDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id, out ItemDetail items))
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
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(UpdateItem item)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var result = await _itemService.Create(await HttpContext.GetAccessToken(), item);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                Created("item/" + result.Data!.Id, result.Message) :
                StatusCode((int)result.Status, result.Message);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, UpdateItem item)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var result = await _itemService.Update(await HttpContext.GetAccessToken(), id, item);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _itemService.Delete(await HttpContext.GetAccessToken(), id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : StatusCode((int)result.Status, result.Message);
        }
    }
}
