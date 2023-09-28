using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Service;
using Inventory.Service.Common;
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

        public async Task<IActionResult> GetPagination([FromQuery] PaginationRequest request)
        {
            var result = await _inUseService.GetPagination(await HttpContext.GetAccessToken(), request);

            return result.Status == ResponseCode.Success ?
                Ok(result) : StatusCode((int)result.Status);
        }
    }
}
