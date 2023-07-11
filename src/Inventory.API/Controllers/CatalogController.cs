using Inventory.Core.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Inventory.Core.Request;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CatalogController : ControllerBase
    {
        private readonly ICatalogServices _catalogServices;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "Inventory.Catalog";

        public CatalogController(ICatalogServices catalogServices, IRedisCacheService cacheService)
        {
            _catalogServices = catalogServices;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PaginationCatalog([FromQuery]PaginationRequest request)
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<CatalogDTO> catalogs)) 
            {
                return Ok(catalogs);
            }
            else
            {
                var result = await _catalogServices.GetPagination(request);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + queryString, result);
                    return StatusCode(200, result);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListCatalog()
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + ".List" + queryString, out IEnumerable<CatalogDTO> catalogs))
            {
                return Ok(catalogs);
            }
            else
            {
                var result = await _catalogServices.GetList();

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + ".List" + queryString, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCatalog(int id)
        {
            if(_cacheService.TryGetCacheAsync(redisKey + "." + id,out CatalogDTO catalog))
            {
                return Ok(catalog);
            }
            else
            {
                var result = await _catalogServices.GetById(id);

                if (result.Status == ResponseStatus.STATUS_SUCCESS)
                {
                    await _cacheService.SetCacheAsync(redisKey + "." + id, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Messages);
            }
        }

        [HttpPost]
        [Authorize(Roles =InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCatalog(CatalogEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.CreateCatalog(dto);

            await _cacheService.RemoveCacheTreeAsync(redisKey);
            
            return Created("catalog/"+result.Data!.Id,result.Messages);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCatalog(int id, CatalogEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.UpdateCatalog(id, dto);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                await _cacheService.RemoveCacheTreeAsync(redisKey);
                return Ok(result.Messages);
            }
            else
            {
                return NotFound(result.Messages);
            }
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = InventoryRoles.IM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCatalog(int id)
        {
            var result = await _catalogServices.DeleteCatalog(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
            {
                await _cacheService.RemoveCacheTreeAsync(redisKey);
                return Ok(result.Messages);
            }
            else
            {
                return NotFound(result.Messages);
            }
        }
    }
}
