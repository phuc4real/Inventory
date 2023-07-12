using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
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
    public class TeamController : ControllerBase
    {
        private readonly ITeamServices _teamServices;
        private readonly IRedisCacheService _cacheService;
        private const string redisKey = "team";

        public TeamController(ITeamServices teamServices, IRedisCacheService cacheService)
        {
            _teamServices = teamServices;
            _cacheService = cacheService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListTeam()
        {
            if (_cacheService.TryGetCacheAsync(redisKey,out IEnumerable<TeamDTO> teams))
            {
                return Ok(teams);
            }
            else 
            {
                var result = await _teamServices.GetList();

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey,result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Message);
            }
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(List<TeamWithMembersDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeam(Guid id)
        {
            if (_cacheService.TryGetCacheAsync(redisKey + id, out TeamWithMembersDTO team))
            {
                return Ok(team);
            }
            else
            {
                var result = await _teamServices.GetById(id);

                if (result.Status == ResponseCode.Success)
                {
                    await _cacheService.SetCacheAsync(redisKey + id, result.Data);
                    return Ok(result.Data);
                }

                return NotFound(result.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam(TeamEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.CreateTeam(token, dto);
            await _cacheService.RemoveCacheAsync(redisKey);

            return Created("team/"+result.Data!.Id,result.Message);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeam(Guid id, TeamEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.UpdateTeam(token, id, dto);

            await _cacheService.RemoveCacheAsync(new[] { redisKey, redisKey + id });

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : NotFound(result.Message);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.DeleteTeam(token, id);

            await _cacheService.RemoveCacheAsync(new[] { redisKey, redisKey + id });

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : NotFound(result.Message);
        }

        [HttpPost("{id:Guid}/add-member")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMembers(Guid id, string memberId)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.AddMember(token, id, memberId);

            await _cacheService.RemoveCacheAsync(redisKey+id);

            return result.Status == ResponseCode.Success ?
                    Ok(result.Message) : NotFound(result.Message);
        }
    }
}
