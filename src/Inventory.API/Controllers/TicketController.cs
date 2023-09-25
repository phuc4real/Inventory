using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service;
using Inventory.Service.Common.Request;
using Inventory.Service.Common.Response;
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
        [ProducesResponseType(typeof(PaginationResponse<Ticket>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var result = await _ticketService.GetPagination(await HttpContext.GetAccessToken(), request);

            return result.Status == ResponseCode.Success ?
                    Ok(result) : NoContent();
        }

        [HttpGet("list")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(List<Ticket>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ListTicket()
        {
            var result = await _ticketService.GetList();

            return result.Status == ResponseCode.Success ?
                    Ok(result.Data) : NoContent();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TicketWithHistory), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _ticketService.GetById(await HttpContext.GetAccessToken(), id);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Data) : StatusCode((int)result.Status, result.Message);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(UpdateTicketInfo dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _ticketService.Create(await HttpContext.GetAccessToken(), dto);

            return result.Status == ResponseCode.Success ?
                    Created("ticket/" + result.Data!.Id, result.Message) :
                    StatusCode((int)result.Status, result.Message);
        }


        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _ticketService.Cancel(await HttpContext.GetAccessToken(), id);

            return StatusCode((int)result.Status, result.Message);
        }

        [HttpPut("{id:int}/update-status")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _ticketService.UpdateStatus(await HttpContext.GetAccessToken(), id);

            return StatusCode((int)result.Status, result.Message);
        }

        [HttpPut("{id:int}/decide")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Decide(int id, UpdateDecision decision)
        {
            var result = await _ticketService.Decide(await HttpContext.GetAccessToken(), id, decision);

            return StatusCode((int)result.Status, result.Message);
        }

        [HttpPut("{id:int}/leader-decide")]
        [Authorize(Roles = InventoryRoles.TeamLeader)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LeaderDecide(int id, UpdateDecision decision)
        {
            var result = await _ticketService.LeaderDecide(await HttpContext.GetAccessToken(), id, decision);

            return StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("count")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(List<Ticket>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicketCount()
        {
            var result = await _ticketService.GetTicketCount();

            return StatusCode((int)result.Status, result.Data);
        }
    }
}
