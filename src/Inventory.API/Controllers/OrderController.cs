using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.IM)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "order";

        public OrderController(IOrderService orderService, IRedisCacheService cacheService)
        {
            _orderService = orderService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListOrder()
        {
            if (_cacheService.TryGetCacheAsync(redisKey,out IEnumerable<OrderDTO> orders))
            {
                return Ok(orders);
            }
            else
            {
                var result = await _orderService.GetAll();

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey, result.Data);
                    return Ok(result.Data);
                }
                return NotFound(result.Messages);
            }
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO dto)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var token = await HttpContext.GetAccessToken();

            var result = await _orderService.CreateOrder(token, dto);

            await _cacheService.RemoveCacheAsync(redisKey);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Created("order/"+result.Data!.Id,result.Messages) : NotFound(result.Messages);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(OrderDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrder(int id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + id,out OrderDTO order))
            {
                return Ok(order);
            }
            else
            {
                var result = await _orderService.GetById(id);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + id, result.Data);
                    return Ok(result.Data);
                }
                return NotFound(result.Messages);
            }
        }

        [HttpPut("{id:int}/update-status")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrderStatus(int id)
        {
            var result = await _orderService.UpdateOrderStatus(id);

            await _cacheService.RemoveCacheAsync(new[] { redisKey, redisKey + id });

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : BadRequest(result.Messages);
        }

        [HttpDelete("{id:int}/cancel")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrder(id);

            await _cacheService.RemoveCacheAsync(new[] { redisKey, redisKey + id });

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : BadRequest(result.Messages);
        }

        [HttpGet("by-item/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> OrdersByItemId(Guid itemId)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + itemId, out IEnumerable<OrderDTO> orders))
            {
                return Ok(orders);
            }
            else
            {
                var result = await _orderService.GetOrdersByItemId(itemId);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + itemId, result.Data);
                    return Ok(result.Data);
                }
                return NotFound(result.Messages);
            }
        }
    }
}
