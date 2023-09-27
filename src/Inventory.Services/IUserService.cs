
using Inventory.Service.DTO.User;

namespace Inventory.Service
{
    public interface IUserService
    {
        public Task<UserListResponse> GetListAsync(string? search);
        public Task<UserObjectResponse> GetByIdAsync(string id);
        public Task<UserObjectResponse> GetAsync(string token);
    }
}
