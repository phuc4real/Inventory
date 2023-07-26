using Inventory.Core.Response;
using Inventory.Core.ViewModel;

namespace Inventory.Services.IServices
{
    public interface IUserService
    {
        Task<ResultResponse<IEnumerable<AppUser>>> GetList(string? filter);
        Task<ResultResponse<AppUserDetail>> GetById(string id);
        Task<ResultResponse<AppUserDetail>> GetByToken(string token);
    }
}
