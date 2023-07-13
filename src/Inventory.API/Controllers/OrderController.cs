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
    [Authorize(Roles = InventoryRoles.IM)]
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
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ListOrder()
        {
            if (_cacheService.TryGetCacheAsync(redisKey + ".ListOrder",out IEnumerable<OrderDTO> orders))
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
        [ProducesResponseType(typeof(PaginationResponse<OrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetOrderPagination([FromQuery] PaginationRequest request)
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<OrderDTO> orders))
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
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO dto)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var token = await HttpContext.GetAccessToken();

            var result = await _orderService.CreateOrder(token, dto);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Created("order/"+result.Data!.Id,result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(OrderDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id,out OrderDTO order))
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
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int id)
        {
            var result = await _orderService.UpdateOrderStatus(id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }

        [HttpDelete("{id:int}/cancel")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrder(id);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }
    }
}
