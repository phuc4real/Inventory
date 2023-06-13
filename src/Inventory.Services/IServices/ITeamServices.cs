using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface ITeamServices
    {
        Task<ResultResponse<IEnumerable<TeamDTO>>> GetAll();
        Task<ResultResponse<TeamWithMembersDTO>> GetById(Guid id);
        Task<ResultResponse<IEnumerable<TeamDTO>>> SearchTeamByName(string name);
        Task<ResultResponse<TeamDTO>> CreateTeam(TeamEditDTO dto);
        Task<ResultResponse<TeamDTO>> UpdateTeam(Guid id, TeamEditDTO dto);
        Task<ResultResponse<TeamDTO>> DeleteTeam(Guid id);
    }
}
