using Inventory.Core.Common;
using Inventory.Core.Extensions;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam(TeamEditDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var result = await _teamServices.CreateTeam(dto);

            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(List<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeam(Guid id, TeamEditDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var result = await _teamServices.UpdateTeam(id, dto);

            return result.Status == ResponseStatus.STATUS_SUCCESS ?
                    Ok(result) : NotFound(result.Messages);
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ResponseMessage>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var result = await _teamServices.DeleteTeam(id);

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
    }
}
