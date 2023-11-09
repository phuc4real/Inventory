using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Extensions;
using Inventory.Service;
using Inventory.Service.Common;
using Inventory.Service.DTO.Item;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ItemPaginationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Pagination([FromQuery] PaginationRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _itemService.GetPaginationAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            else
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ItemObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ItemObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            if (ModelState.IsValid)
            {
                ItemRequest request = new() { Id = id };
                request.SetContext(HttpContext);
                var result = await _itemService.GetByIdAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            else
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
        }

        [HttpGet("{id}/compact")]
        [ProducesResponseType(typeof(ItemCompactResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ItemCompactResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompactItem(int id)
        {
            if (ModelState.IsValid)
            {
                ItemRequest request = new() { Id = id };
                request.SetContext(HttpContext);
                var result = await _itemService.GetByIdCompactAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            else
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ItemObjectResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ItemObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(ItemUpdateRequest request)
        {
            if (ModelState.IsValid)
            {
                request.SetContext(HttpContext);
                var result = await _itemService.CreateAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            else
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(ItemObjectResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ItemObjectResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, ItemUpdateRequest request)
        {
            if (ModelState.IsValid)
            {
                request.Id = id;
                request.SetContext(HttpContext);
                var result = await _itemService.UpdateAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            else
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = InventoryRoles.Admin)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResultMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                ItemRequest request = new() { Id = id };
                request.SetContext(HttpContext);
                var result = await _itemService.DeactiveAsync(request);

                return StatusCode((int)result.StatusCode, result);
            }
            else
            {
                return BadRequest(ModelState.GetErrorMessages());
            }
        }
    }
}
