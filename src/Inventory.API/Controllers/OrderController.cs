using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Service;
using Inventory.Service.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.Admin)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "Inventory.Order";

        public OrderController(IOrderService orderService, IRedisCacheService cacheService)
        {
            _orderService = orderService;
            _cacheService = cacheService;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> List()
        {
            if (_cacheService.TryGetCacheAsync(redisKey + ".ListOrder", out IEnumerable<Order> orders))
            {
                return Ok(orders);
            }
            else
            {
                var result = await _orderService.GetList();

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + ".ListOrder", result.Data);
                    return Ok(result.Data);
                }
                return StatusCode((int)result.Status);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<Order> orders))
            {
                return Ok(orders);
            }
            else
            {
                var result = await _orderService.GetPagination(request);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + queryString, result);
                    return Ok(result);
                }
                return StatusCode((int)result.Status);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(UpdateOrderInfo dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var result = await _orderService.Create(await HttpContext.GetAccessToken(), dto);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Created("order/" + result.Data!.Id, result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderWithHistory), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id, out OrderWithHistory order))
            {
                return Ok(order);
            }
            else
            {
                var result = await _orderService.GetById(id);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + "." + id, result.Data);
                    return Ok(result.Data);
                }
                return StatusCode((int)result.Status, result.Message);
            }
        }

        [HttpPut("{id:int}/update-status")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _orderService.UpdateStatus(await HttpContext.GetAccessToken(), id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }


        [HttpPost("{id:int}/decide")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Decide(int id, UpdateDecision decision)
        {
            var result = await _orderService.Decide(await HttpContext.GetAccessToken(), id, decision);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }

        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _orderService.Cancel(await HttpContext.GetAccessToken(), id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }


        [HttpGet("count-by-month")]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            var result = await _orderService.GetCountByMonth();

            return StatusCode((int)result.Status, result.Data);
        }
    }
}
