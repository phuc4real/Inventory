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
    public class ExportController : ControllerBase
    {
        private readonly IExportService _exportService;

        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ExportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _exportService.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS
                    ? Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("byitem/{itemId:Guid}")]
        [ProducesResponseType(typeof(List<ExportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExportByItemId(Guid itemId)
        {
            var result = await _exportService.GetExportByItemId(itemId);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ExportDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExport(int id)
        {
            var result = await _exportService.GetById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }


        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse<ExportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateExport(ExportCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var token = await HttpContext.GetAccessToken();

            var result = await _exportService.CreateExport(token, dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }
    }
}
