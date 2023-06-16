using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IItemService _itemService;

        public TicketController(ITicketService ticketService, IItemService itemService)
        {
            _ticketService = ticketService;
            _itemService = itemService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TicketDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListTicket() 
        {
            var result = await _ticketService.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(TicketDTO),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTicketById(Guid id)
        {
            var result = await _ticketService.GetTicketById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("by-item/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TicketsByItemId(Guid itemId)
        {
            var result = await _ticketService.TicketsByItemId(itemId);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTicket(TicketCreateDTO dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.CreateTicket(token, dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }


        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTicketInfo(Guid id, TicketCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.UpdateTicketInfo(token,id ,dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpPut("{id:Guid}/update-status")]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.UpdateStatus(token, id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : BadRequest(result.Messages);
        }

        [HttpPut("{id:Guid}/reject")]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectTicket(Guid id, string rejectReason)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.RejectTicket(token ,id ,rejectReason);
                
            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Ok(result) : NotFound(result.Messages);
        }

        [HttpPut("{id:Guid}/pm-approve")]
        [HttpPut("{id:Guid}/pm-reject")]
        [ProducesResponseType(typeof(ResultResponse<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PMStatus(Guid id, string? rejectReason)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _ticketService.PMStatus(token, id, rejectReason);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Ok(result) : NotFound(result.Messages);
        }


        [HttpGet("search")]
        [ProducesResponseType(typeof(List<TicketDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchTicket(string filter)
        {
            var result = await _ticketService.SearchTicket(filter);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Ok(result.Data) : NotFound(result.Messages);
        }
    }
}
