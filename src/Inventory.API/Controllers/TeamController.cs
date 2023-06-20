using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly ITeamServices _teamServices;

        public TeamController(ITeamServices teamServices)
        {
            _teamServices = teamServices;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListTeam()
        {
            var result = await _teamServices.GetAll();

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(List<TeamWithMembersDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamById(Guid id)
        {
            var result = await _teamServices.GetById(id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }

        [HttpPost]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam(TeamEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorMessages());
            }

            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.CreateTeam(token, dto);

            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
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

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.DeleteTeam(token, id);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : NotFound(result.Messages);
        }


        [HttpGet("search")]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByName(string name)
        {
            var result = await _teamServices.SearchTeamByName(name);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Data) : NotFound(result.Messages);
        }


        [HttpPost("{id:Guid}/add-member")]
        [Authorize(Roles = InventoryRoles.PM)]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMembers(Guid id, string memberId)
        {
            var token = await HttpContext.GetAccessToken();

            var result = await _teamServices.AddMember(token, id, memberId);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result.Messages) : NotFound(result.Messages);
        }
    }
}
