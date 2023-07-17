using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.IServices
{
    public interface IUserService
    {
        Task<ResultResponse<IEnumerable<AppUserDTO>>> GetList();
        Task<PaginationResponse<AppUserDTO>> GetPagination(PaginationRequest request);
        Task<ResultResponse<AppUserWithTeamDTO>> GetById(string id);
    }
}
