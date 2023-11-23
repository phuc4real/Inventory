using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Extensions;
using Inventory.Service;
using Inventory.Service.Common;
using Inventory.Service.DTO.Export;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ExportPaginationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _exportService.GetPaginationAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExportObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(int id)
        {
            var request = new ExportRequest { Id = id };
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _exportService.GetByIdAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("{id}/entry")]
        [ProducesResponseType(typeof(ExportObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntries(int id)
        {
            var request = new ExportRequest { Id = id };
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _exportService.GetEntriesAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPut("{id}/update-status")]
        [Authorize(Roles = InventoryRoles.SuperAdmin)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            var request = new ExportRequest { Id = id };
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _exportService.UpdateExportStatusAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("chart")]
        [ProducesResponseType(typeof(ChartDataResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportDataChart()
        {
            return StatusCode(200, await _exportService.GetChartDataAsync());
        }
    }
}
