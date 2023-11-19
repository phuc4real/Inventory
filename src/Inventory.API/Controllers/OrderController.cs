using Inventory.Core.Common;
using Inventory.Core.Constants;
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
    [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
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
                var result = await _orderService.CreateOrUpdateAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());

        }

        [HttpGet("{recordId}")]
        [ProducesResponseType(typeof(OrderObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OrderObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int recordId)
        {
            if (ModelState.IsValid)
            {
                var request = new OrderRequest { RecordId = recordId };
                request.SetContext(HttpContext);
                var result = await _orderService.GetByIdAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }


        [HttpGet("{recordId}/entry")]
        [ProducesResponseType(typeof(OrderEntryListResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OrderEntryListResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrderEntry(int recordId)
        {
            if (ModelState.IsValid)
            {
                var request = new OrderRequest { RecordId = recordId };
                request.SetContext(HttpContext);
                var result = await _orderService.GetOrderEntries(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPut("{orderId}/update-status")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(int orderId)
        {
            if (ModelState.IsValid)
            {
                var request = new OrderRequest { OrderId = orderId };
                request.SetContext(HttpContext);
                var result = await _orderService.UpdateOrderStatusAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpDelete("{orderId}/cancel")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int orderId)
        {
            if (ModelState.IsValid)
            {
                var request = new OrderRequest { OrderId = orderId };
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
