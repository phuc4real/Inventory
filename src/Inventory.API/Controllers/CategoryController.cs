using Inventory.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Inventory.Core.Enums;
using Inventory.Service;
using Inventory.Service.DTO.Category;
using Azure.Core;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _catalogServices;

        public CategoryController(ICategoryService catalogServices)
        {
            _catalogServices = catalogServices;
        }

        [HttpGet]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(CategoryPaginationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            request.SetContext(HttpContext);

            var result = await _catalogServices.GetPaginationAsync(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(CategoryListResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            Core.Common.Request request = new ore.Common.Request();
            request.SetContext(HttpContext);

            var result = await _catalogServices.GetListAsync();

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Catalog), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + "." + id, out Catalog catalog))
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
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(UpdateCatalog dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.Create(dto);

            await _cacheService.RemoveCacheTreeAsync(redisKey);

            return Created("catalog/" + result.Data!.Id, result.Message);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, UpdateCatalog dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var result = await _catalogServices.Update(id, dto);

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
            var result = await _catalogServices.Delete(id);

            if (result.Status == ResponseCode.Success)

                await _cacheService.RemoveCacheTreeAsync(redisKey);

            return StatusCode((int)result.Status, result.Message);
        }
    }
}
