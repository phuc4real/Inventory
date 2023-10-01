using Inventory.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Inventory.Core.Enums;
using Inventory.Service;
using Inventory.Service.DTO.Category;
using Azure.Core;
using Inventory.Service.Common;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(CategoryPaginationResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            request.SetContext(HttpContext);

            var result = await _categoryService.GetPaginationAsync(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CategoryUpdateRequest request)
        {
            request.SetContext(HttpContext);

            var result = await _categoryService.CreateAsync(request);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, CategoryUpdateRequest request)
        {
            request.SetContext(HttpContext);

            var result = await _categoryService.UpdateAsync(id, request);

            return StatusCode((int)result.StatusCode, result);
        }


        [HttpDelete("{id:int}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new BaseRequest();
            request.SetContext(HttpContext);

            var result = await _categoryService.DeactiveAsync(id, request);

            return StatusCode((int)result.StatusCode, result);
        }
    }
}
