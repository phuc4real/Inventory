using Inventory.Core.ViewModel;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogServices _catalogServices;

        public CatalogController(ICatalogServices catalogServices)
        {
            _catalogServices = catalogServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ListCatalog()
        {
            var result = await _catalogServices.GetAll();

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Data);
            else
                return BadRequest(result.Messages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCatalog(int id)
        {
            var result = await _catalogServices.GetById(id);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Data);
            else
                return BadRequest(result.Messages);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CatalogByName(string value)
        {
            var result = await _catalogServices.SearchCatalog(value);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Data);
            else
                return BadRequest(result.Messages);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultResponse<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCatalog(string catalogName)
        {
            var result = await _catalogServices.CreateCatalog(catalogName);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result);
            else
                return BadRequest(result.Messages);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ResultResponse<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCatalog(int id, string name)
        {
            var result = await _catalogServices.UpdateCatalog(id, name);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result);
            else
                return BadRequest(result.Messages);
        }


        [HttpDelete("{id:int}")]

        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCatalog(int id)
        {
            var result = await _catalogServices.DeleteCatalog(id);

            if (result.Status == ResponeStatus.STATUS_SUCCESS)
                return Ok(result.Messages);
            else
                return BadRequest(result.Messages);
        }
    }
}
