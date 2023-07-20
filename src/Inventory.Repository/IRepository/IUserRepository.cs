using Inventory.Core.Request;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Repository.IRepository
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<PaginationList<AppUser>> GetPagination(PaginationRequest request);
        Task<IEnumerable<AppUser>> GetList();
        Task<AppUser> GetById(string id);
        Task<RoleDTO> CheckRole(string userId);
    }
}
