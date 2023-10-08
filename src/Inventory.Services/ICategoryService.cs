using Inventory.Core.Common;
using Inventory.Service.Common;
using Inventory.Service.DTO.Category;

namespace Inventory.Service
{
    public interface ICategoryService
    {
        public Task<CategoryPaginationResponse> GetPaginationAsync(PaginationRequest request);
        public Task<CategoryObjectResponse> GetByIdAsync(CategoryRequest request);
        public Task<CategoryObjectResponse> CreateAsync(CategoryUpdateRequest request);
        public Task<CategoryObjectResponse> UpdateAsync(CategoryUpdateRequest request);
        public Task<BaseResponse> DeactiveAsync(CategoryRequest request);
    }
}
