using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var result = await _ticketService.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }
    }
}
