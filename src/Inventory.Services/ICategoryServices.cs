using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Category;

namespace Inventory.Service
{
    public interface ICategoryServices
    {
        public Task<CategoryListResponse> GetListAsync();
        public Task<CategoryPaginationResponse> GetPaginationAsync(PaginationRequest request);
        public Task<CategoryObjectResponse> GetByIdAsync(int id);
        public Task<CategoryObjectResponse> CreateAsync(CategoryUpdateRequest request);
        public Task<CategoryObjectResponse> UpdateAsync(int id, CategoryUpdateRequest request);
        public Task<BaseResponse> DeactiveAsync(int id);
    }
}
