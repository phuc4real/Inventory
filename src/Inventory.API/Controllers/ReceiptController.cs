using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.IM)]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "receipt";

        public ReceiptController(IReceiptService receiptService, IRedisCacheService cacheService)
        {
            _receiptService = receiptService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ReceiptDTO>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListReceipt() 
        {
            if (_cacheService.TryGetCacheAsync(redisKey,out IEnumerable<ReceiptDTO> receipts))
            {
                return Ok(receipts);
            }
            else
            {
                var result = await _receiptService.GetAll();
                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ReceiptDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReceiptById(int id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + id,out ReceiptDTO receipt))
            {
                return Ok(receipt);
            }
            else
            {
                var result = await _receiptService.ReceiptById(id);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + id, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("by-item/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<ReceiptDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReceiptsByItemId(Guid itemId)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + itemId, out IEnumerable<ReceiptDTO> receipts))
            {
                return Ok(receipts);
            }
            else
            {
                var result = await _receiptService.ReceiptByItemId(itemId);
                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + itemId, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateReceipt(ReceiptCreateDTO dto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _receiptService.CreateReceipt(token, dto);

            await _cacheService.RemoveCacheAsync(redisKey);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Created("receipt/"+result.Data!.Id,result.Messages) : NotFound(result.Messages);
        }
    }
}
