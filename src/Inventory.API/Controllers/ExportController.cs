using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.Admin)]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "Inventory.Export";

        public ExportController(IExportService exportService, IRedisCacheService cacheService)
        {
            _exportService = exportService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<Export>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<Export> catalogs))
            {
                return Ok(catalogs);
            }
            else
            {
                var result = await _exportService.GetPagination(request);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + queryString, result);
                    return Ok(result);
                }

                return StatusCode((int)result.Status);
            }
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<Export>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> List()
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + ".List" + queryString, out IEnumerable<Export> exports))
            {
                return Ok(exports);
            }
            else
            {
                var result = await _exportService.GetList();

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + ".List" + queryString, result.Data);
                    return Ok(result.Data);
                }

                return StatusCode((int)result.Status);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Export), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id, out Export export))
            {
                return Ok(export);
            }
            else
            {
                var result = await _exportService.GetById(id);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + "." + id, result.Data);
                    return Ok(result.Data);
                }

                return StatusCode((int)result.Status, result.Message);
            }
        }

        [HttpDelete("{id:int}/update-status")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var result = await _exportService.UpdateStatus(await HttpContext.GetAccessToken(), id);
            await _cacheService.RemoveCacheTreeAsync(redisKey);
            return StatusCode((int)result.Status, result.Message);
        }

        [HttpGet("count-by-month")]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            var result = await _exportService.GetCountByMonth();

            return StatusCode((int)result.Status, result.Data);
        }
    }
}
