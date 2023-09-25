using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Service
{
    public interface IUserService
    {
        Task<ResultResponse<IEnumerable<AppUsers>>> GetList(string? filter);
        Task<ResultResponse<AppUserDetail>> GetById(string id);
        Task<ResultResponse<AppUserDetail>> GetByToken(string token);
    }
}
