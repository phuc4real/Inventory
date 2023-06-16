using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ReceiptDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListReceipt() 
        {
            var result = await _receiptService.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReceiptById(int id)
        {
            var result = await _receiptService.ReceiptById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("by-item/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<ReceiptDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReceiptsByItemId(Guid itemId)
        {
            var result = await _receiptService.ReceiptByItemId(itemId);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<ReceiptDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateReceipt(ReceiptCreateDTO dto)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var token = await HttpContext.GetAccessToken();

            var result = await _receiptService.CreateReceipt(token, dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Ok(result.Data) : NotFound(result.Messages);
        }
    }
}
