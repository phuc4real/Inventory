using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
namespace Inventory.Services.IServices
{
    public interface IUserService
    {
        Task<ResultResponse<IEnumerable<AppUserDTO>>> GetList();
        Task<PaginationResponse<AppUserDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<AppUserWithTeamDTO>> GetById(string id);
        Task<ResultResponse<AppUserDTO>> GrantPermission(GrantRoleDTO dto);
        Task<ResultResponse<RoleDTO>> CheckRole(string id);
        Task<ResultResponse<AppUserDTO>> GetUserInfo(string token);
    }
}
