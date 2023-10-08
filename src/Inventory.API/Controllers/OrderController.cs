using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Service;
using Inventory.Service.Common;
using Inventory.Service.DTO.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.Admin)]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(OrderPageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OrderPageResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _orderService.GetPaginationAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPost]
        [ProducesResponseType(typeof(OrderObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OrderObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(OrderUpdateRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _orderService.CreateAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());

        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(OrderObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OrderObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(OrderRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _orderService.GetByIdAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPut("{id:int}/update-status")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(OrderRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _orderService.UpdateOrderStatusAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(OrderRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _orderService.CancelOrderAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }


        [HttpGet("chart")]
        [ProducesResponseType(typeof(ChartDataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            return StatusCode(200, await _orderService.GetOrderChartAsync());
        }
    }
}
