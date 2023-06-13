using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListOrder()
        {
            var result = await _orderService.GetAll();

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder(OrderCreateDTO dto)
        {
            if(!ModelState.IsValid) { return BadRequest(ModelState); }

            var result = await _orderService.CreateOrder(await GetToken(),dto);

            if(result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else
            { return BadRequest(result.Messages); }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetById(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else { return BadRequest(result.Messages); }
        }

        [HttpPut("updatestatus/{id:int}")]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _orderService.UpdateStatus(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else { return BadRequest(result.Messages); }
        }

        [HttpDelete("cancel/{id:int}")]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var result = await _orderService.CancelOrder(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else { return BadRequest(result.Messages); }
        }

        [HttpGet("byitem/{itemId:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<OrderDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> OrdersByItemId(Guid itemId)
        {
            var result = await _orderService.GetOrdersByItemId(itemId);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
                return Ok(result);
            else
            { return BadRequest(result.Messages); }
        }

#pragma warning disable CS8603 // Possible null reference return.
        private async Task<string> GetToken() => await HttpContext.GetTokenAsync("access_token");
#pragma warning restore CS8603 // Possible null reference return.
    }
}
