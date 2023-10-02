using Azure.Core;
using Inventory.Core.Common;
using Inventory.Service;
using Inventory.Service.Common;
using Inventory.Service.DTO.Export;
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
        [ProducesResponseType(typeof(ExportPaginationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            request.SetContext(HttpContext);

            var result = await _exportService.GetPaginationAsync(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ExportObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExportObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _exportService.GetByIdAsync(id);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id:int}/update-status")]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            BaseRequest request = new();
            request.SetContext(HttpContext);

            var result = await _exportService.UpdateExportStatusAsync(id, request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("chart-data")]
        [ProducesResponseType(typeof(ExportChartDataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            return StatusCode(200, await _exportService.GetChartDataAsync());
        }
    }
}
