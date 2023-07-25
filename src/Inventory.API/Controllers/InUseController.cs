﻿using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InUseController : ControllerBase
    {
        private readonly IInUseService _inUseService;

        public InUseController(IInUseService inUseService)
        {
            _inUseService = inUseService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginationResponse<InUse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var result = await _inUseService.GetPagination(await HttpContext.GetAccessToken(), request);

            return result.Status == ResponseCode.Success ?
                Ok(result) : StatusCode((int)result.Status);
        }
    }
}
