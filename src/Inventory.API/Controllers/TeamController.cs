using Inventory.Core.Common;
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
        [ProducesResponseType(typeof(IEnumerable<TeamDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListTeam()
        {
            var result = await _teamServices.GetAll();
            
            return Ok(result.Data);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(IEnumerable<TeamWithMembersDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamById(Guid id)
        {
            var result = await _teamServices.GetById(id);


            if (result.Status == ResponseStatus.STATUS_SUCCESS)
                return Ok(result.Data);
            else
                return NotFound(result.Messages);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> CreateTeam(TeamEditDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _teamServices.CreateTeam(dto);

            return Ok(result);
        }

        [HttpPut("{id:Guid}")]
        [ProducesResponseType(typeof(IEnumerable<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTeam(Guid id, TeamEditDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _teamServices.UpdateTeam(id, dto);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
                return Ok(result);
            else
                return NotFound(result.Messages);
        }

        [HttpDelete("{id:Guid}")]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid id)
        {
            var result = await _teamServices.DeleteTeam(id);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
                return Ok(result.Messages);
            else
                return NotFound(result.Messages);
        }


        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseMessage), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByName(string name)
        {
            var result = await _teamServices.SearchTeamByName(name);

            if (result.Status == ResponseStatus.STATUS_SUCCESS)
                return Ok(result.Data);
            else
                return NotFound(result.Messages);
        }
    }
}
