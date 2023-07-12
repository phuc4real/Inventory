using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
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
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TicketDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListTicket() 
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.GetList(token);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Data) : NotFound(result.Message);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(TicketDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTicketById(Guid id)
        {
            var token = await HttpContext.GetAccessToken();
            var result = await _ticketService.GetById(token, id);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Data) : NotFound(result.Message);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTicket(TicketCreateDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.CreateTicket(token, dto);

            return result.Status == ResponseCode.Success ?
                    Ok(result) : NotFound(result.Message);
        }


        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTicketInfo(Guid id, TicketCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.UpdateTicketInfo(token,id ,dto);

            return result.Status == ResponseCode.Success ?
                    Ok(result) : NotFound(result.Message);
        }

        [HttpPut("{id:Guid}/update-status")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.UpdateStatus(token, id);

            return result.Status == ResponseCode.Success ?
                    Ok(result) : BadRequest(result.Message);
        }

        [HttpPut("{id:Guid}/reject")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectTicket(Guid id, string rejectReason)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.RejectTicket(token ,id ,rejectReason);
                
            return result.Status == ResponseCode.Success ?
                Ok(result) : NotFound(result.Message);
        }

        [HttpPut("{id:Guid}/pm-approve")]
        [HttpPut("{id:Guid}/pm-reject")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PMStatus(Guid id, string? rejectReason)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.UpdatePMStatus(token, id, rejectReason);

            return result.Status == ResponseCode.Success ?
                Ok(result) : NotFound(result.Message);
        }

        [HttpGet("my-ticket")]
        [ProducesResponseType(typeof(List<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MyTicket()
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.GetMyTickets(token);

            return result.Status == ResponseCode.Success ?
                Ok(result.Data) : NotFound(result.Message);
        }
    }
}
