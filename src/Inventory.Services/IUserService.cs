
using Inventory.Core.Common;
using Inventory.Service.DTO.User;

namespace Inventory.Service
{
    public interface IUserService
    {
        public Task<UserPaginationResponse> GetListAsync(PaginationRequest request);
        public Task<UserObjectResponse> GetByIdAsync(string id);
        public Task<UserObjectResponse> GetAsync(BaseRequest request);
        public Task<UserPermission> CheckRoleOfUser(string userId);
    }
}
