
using Inventory.Core.Common;
using Inventory.Model.Entity;
using Inventory.Service.DTO.User;

namespace Inventory.Service
{
    public interface IUserService
    {
        public Task<UserPaginationResponse> GetListAsync(PaginationRequest request);
        public Task<UserObjectResponse> GetByUserNameAsync(string userName);
        public Task<UserObjectResponse> GetAsync(BaseRequest request);
        public Task<UserPermission> CheckRoleOfUser(string userId);
        public Task<Operation> GetOperationAsync(BaseRequest request);
        public Task<UserPaginationResponse> GetSuperAdminListAsync();
    }
}
