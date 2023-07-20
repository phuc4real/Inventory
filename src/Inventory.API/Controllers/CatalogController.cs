using Inventory.Core.ViewModel;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Inventory.Core.Request;
using Inventory.Core.Enums;

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
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(PaginationResponse<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + queryString, out PaginationResponse<CatalogDTO> catalogs)) 
            {
                return Ok(catalogs);
            }
            else
            {
                var result = await _catalogServices.GetPagination(request);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + queryString, result);
                    return Ok(result);
                }

                return StatusCode((int)result.Status);
            }
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<CatalogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> List()
        {
            var queryString = Request.QueryString.ToString();

            if (_cacheService.TryGetCacheAsync(redisKey + ".List" + queryString, out IEnumerable<CatalogDTO> catalogs))
            {
                return Ok(catalogs);
            }
            else
            {
                var result = await _catalogServices.GetList();

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + ".List" + queryString, result.Data);
                    return Ok(result.Data);
                }

                return StatusCode((int)result.Status);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CatalogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            if(_cacheService.TryGetCacheAsync(redisKey + "." + id,out CatalogDTO catalog))
            {
                return Ok(catalog);
            }
            else
            {
                var result = await _catalogServices.GetById(id);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + "." + id, result.Data);
                    return Ok(result.Data);
                }

                return StatusCode((int)result.Status, result.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles =InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CatalogEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.CreateCatalog(dto);

            await _cacheService.RemoveCacheTreeAsync(redisKey);
            
            return Created("catalog/"+result.Data!.Id,result.Message);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CatalogEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.UpdateCatalog(id, dto);

            if (result.Status == ResponseCode.Success)
                await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _catalogServices.DeleteCatalog(id);

            if (result.Status == ResponseCode.Success)

                await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }
    }
}
