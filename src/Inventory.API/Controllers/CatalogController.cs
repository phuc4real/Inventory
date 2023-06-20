using Inventory.Core.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogServices _catalogServices;

        public CatalogController(ICatalogServices catalogServices)
        {
            _catalogServices = catalogServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListCatalog()
        {
            var result = await _catalogServices.GetAll();
            
            return result.Status == ResponseStatus.STATUS_SUCCESS ? 
                Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCatalog(int id)
        {
            var result = await _catalogServices.GetById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CatalogByName(string value)
        {
            var result = await _catalogServices.SearchCatalog(value);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPost]
        [Authorize(Roles =InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResultResponse<CatalogDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCatalog(CatalogEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.CreateCatalog(dto);
            
            return Created("catalog/"+result.Data!.Id,result.Messages);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResultResponse<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCatalog(int id, CatalogEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.UpdateCatalog(id, dto);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result.Messages);
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCatalog(int id)
        {
            var result = await _catalogServices.DeleteCatalog(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                return Ok(result.Messages);
            }
            else
            {
                return NotFound(result.Messages);
            }
        }
    }
}
