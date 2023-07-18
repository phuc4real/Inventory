using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Inventory.Services.Services;
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
        private const string redisKey = "Inventory.Export";

        public ExportController(IExportService exportService, IRedisCacheService cacheService)
        {
            _exportService = exportService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<ExportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<ExportDTO> catalogs))
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
        [ProducesResponseType(typeof(IEnumerable<ExportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ListExport()
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + ".List" + queryString, out IEnumerable<ExportDTO> exports))
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
        [ProducesResponseType(typeof(ExportDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExport(int id)
        {
            if(_cacheService.TryGetCacheAsync(redisKey + "." + id,out ExportDTO export))
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

        [HttpPost]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateExport(ExportCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _exportService.CreateExport(token, dto);
            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return result.Status == ResponseCode.Success ?
                    Created("export/" + result.Data!.Id, result.Message) : StatusCode((int)result.Status, result.Message);
        }

        [HttpDelete("{id:int}/cancel")]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelExport(int id)
        {
            var result = await _exportService.CancelExport(id);
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
