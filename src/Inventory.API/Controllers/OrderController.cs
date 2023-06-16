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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListOrder()
        {
            var result = await _orderService.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO dto)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState.GetErrorMessages()); }

            var token = await HttpContext.GetAccessToken();

            var result = await _orderService.CreateOrder(token, dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPut("{id:int}/update-status")]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _orderService.UpdateStatus(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : BadRequest(result.Messages);
        }

        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrder(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : BadRequest(result.Messages);
        }

        [HttpGet("by-item/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> OrdersByItemId(Guid itemId)
        {
            var result = await _orderService.GetOrdersByItemId(itemId);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }
    }
}
