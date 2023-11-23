using Azure.Core;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Service;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using Inventory.Service.DTO.Order;
using Inventory.Service.DTO.Ticket;
using Inventory.Service.Implement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(TicketPageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TicketPageResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _ticketService.GetPaginationAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("{recordId}")]
        [ProducesResponseType(typeof(TicketObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TicketObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int recordId)
        {
            var request = new TicketRequest()
            {
                RecordId = recordId
            };
            request.SetContext(HttpContext);

            if (ModelState.IsValid)
            {
                var result = await _ticketService.GetByIdAsync(request);
                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPost]
        [ProducesResponseType(typeof(TicketObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TicketObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(TicketUpdateResquest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _ticketService.CreateOrUpdateAsync(request);
                return StatusCode((int)result.StatusCode, result);
            }
            else
                return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("{recordId}/entry")]
        [ProducesResponseType(typeof(TicketEntryList), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TicketEntryList), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrderEntry(int recordId)
        {
            if (ModelState.IsValid)
            {
                var request = new TicketRequest { RecordId = recordId };
                request.SetContext(HttpContext);
                var result = await _ticketService.GetTicketEntries(request);

                return StatusCode((int)result.StatusCode, result);
            }

            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpDelete("{ticketId}/cancel")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int ticketId)
        {
            var request = new TicketRequest()
            {
                TicketId = ticketId
            };
            request.SetContext(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _ticketService.CancelAsync(request);
                return StatusCode((int)result.StatusCode, result);
            }
            else
                return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPost("{recordId}/approval")]
        [Authorize(Roles = InventoryRoles.SuperAdmin)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Approval(int recordId, CreateCommentRequest request)
        {
            request.SetContext(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _ticketService.ApprovalTicketAsync(recordId, request);
                return StatusCode((int)result.StatusCode, result);
            }
            else
                return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPut("{ticketId}/update-status")]
        [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(int ticketId)
        {
            var request = new TicketRequest()
            {
                TicketId = ticketId
            };
            request.SetContext(HttpContext);
            if (ModelState.IsValid)
            {
                var result = await _ticketService.UpdateStatusAsync(request);
                return StatusCode((int)result.StatusCode, result);
            }
            else
                return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("summary")]
        [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
        [ProducesResponseType(typeof(ChartDataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSummary()
        {
            return StatusCode(200, await _ticketService.GetTicketSummary());

        }

        [HttpGet("type")]
        [ProducesResponseType(typeof(TicketTypeList), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicketType()
        {
            return StatusCode(200, await _ticketService.GetTicketType());
        }


    }
}
