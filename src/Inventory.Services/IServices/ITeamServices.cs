using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface ITeamServices
    {
        Task<ResultResponse<IEnumerable<TeamDTO>>> GetList();
        Task<PaginationResponse<TeamDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<TeamWithMembersDTO>> GetById(Guid id);
        Task<ResultResponse<TeamDTO>> CreateTeam(string token, TeamEditDTO dto);
        Task<ResultResponse<TeamDTO>> UpdateTeam(string token, Guid id, TeamEditDTO dto);
        Task<ResultResponse<TeamDTO>> DeleteTeam(string token, Guid id);
        Task<ResultResponse<TeamDTO>> AddMember(string token, Guid teamId, string memberId);
    }
}
