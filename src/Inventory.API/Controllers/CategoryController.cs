﻿using Inventory.Core.Common;
using Microsoft.AspNetCore.Mvc;
using Inventory.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Inventory.Service;
using Inventory.Service.DTO.Category;
using Inventory.Service.Common;
using Inventory.Core.Constants;

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
        [ProducesResponseType(typeof(CategoryPaginationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _categoryService.GetPaginationAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(CategoryRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _categoryService.GetByIdAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CategoryUpdateRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _categoryService.CreateAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }

        [HttpPut("{id}")]
        [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CategoryObjectResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(CategoryUpdateRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _categoryService.UpdateAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = InventoryRoles.AdminOrSuperAdmin)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var request = new CategoryRequest { Id = id };
                request.SetContext(HttpContext);
                var result = await _categoryService.DeactiveAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            return BadRequest(ModelState.GetErrorMessages());
        }
    }
}
