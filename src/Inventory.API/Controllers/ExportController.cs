using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.IM)]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "export";

        public ExportController(IExportService exportService, IRedisCacheService cacheService)
        {
            _exportService = exportService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ExportWithDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListExport()
        {
            if (_cacheService.TryGetCacheAsync(redisKey, out IEnumerable<ExportWithDetailDTO> exports))
            {
                return Ok(exports);
            }
            else
            {
                var result = await _exportService.GetAll();

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("by-item/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<ExportWithDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListExportByItemId(Guid itemId)
        {
            if(_cacheService.TryGetCacheAsync(redisKey + itemId, out IEnumerable<ExportWithDetailDTO> exports))
            {
                return Ok(exports);
            }
            else
            {
                var result = await _exportService.GetExportByItemId(itemId);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + itemId, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ExportWithDetailDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExport(int id)
        {
            if(_cacheService.TryGetCacheAsync(redisKey + id,out ExportWithDetailDTO export))
            {
                return Ok(export);
            }
            else
            {
                var result = await _exportService.GetById(id);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + id, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateExport(ExportCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _exportService.CreateExport(token, dto);
            await _cacheService.RemoveCacheAsync(redisKey);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Created("export/" + result.Data!.Id, result.Messages) : NotFound(result.Messages);
        }

        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelExport(int id)
        {
            var result = await _exportService.CancelExport(id);
            await _cacheService.RemoveCacheAsync(new [] { redisKey, redisKey + id });
            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : NotFound(result.Messages);
        }
    }
}
