using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> ListCatalog()
        {
            return Ok(await _catalogServices.GetAll());
        }
    }
}
