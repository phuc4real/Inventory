using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Core.Enums;
using Inventory.Repository;
using Inventory.Service.DTO.Category;
using Microsoft.EntityFrameworkCore;
using Inventory.Service.Common;
using Inventory.Core.Common;
using Inventory.Core.Extensions;
using System.Linq;
using Inventory.Core.Const;

namespace Inventory.Service.Implement
{
    public class CategoryService : BaseService, ICategoryService
    {

        #region Ctor & Field

        public CategoryService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
            : base(repoWrapper, mapper, cacheService)
        {
        }

        #endregion

        #region Method

        public async Task<CategoryObjectResponse> GetByIdAsync(CategoryRequest request)
        {
            var cacheKey = CacheNameConstant.Category + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out CategoryObjectResponse response))
            {
                return response;
            };

            response = new CategoryObjectResponse();

            var category = await _repoWrapper.Category.FindByCondition(x => x.Id == request.Id)
                                                    .FirstOrDefaultAsync();
            if (category == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Category", "Not found!");
                return response;
            }

            response.Data = _mapper.Map<CategoryResponse>(category);

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<CategoryObjectResponse> CreateAsync(CategoryUpdateRequest request)
        {
            CategoryObjectResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            request.Id = null;

            Category cate = _mapper.Map<Category>(request);

            await _repoWrapper.Category.AddAsync(cate);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<CategoryResponse>(cate);

            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.CategoruPagination);
            return response;
        }

        public async Task<CategoryObjectResponse> UpdateAsync(CategoryUpdateRequest request)
        {
            CategoryObjectResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var category = await _repoWrapper.Category.FindByCondition(x => x.Id == request.Id)
                                                    .FirstOrDefaultAsync();

            if (category == null || category.IsInactive)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Category", "Category not exists!");
                return response;

            }

            category.Name = request.Name;
            category.Description = request.Description;
            _repoWrapper.Category.Update(category);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<CategoryResponse>(category);

            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.Category + category.Id);
            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.CategoruPagination);
            return response;
        }

        public async Task<BaseResponse> DeactiveAsync(CategoryRequest request)
        {
            BaseResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var category = await _repoWrapper.Category.FindByCondition(x => x.Id == request.Id)
                                                      .FirstOrDefaultAsync();

            if (category == null || category.IsInactive)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Category", "Category not exists!");
                return response;
            }

            _repoWrapper.Category.Remove(category);
            await _repoWrapper.SaveAsync();

            response.Message = new("Category", "Category deleted!");
            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.Category + category.Id);
            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.CategoruPagination);
            return response;
        }

        public async Task<CategoryPaginationResponse> GetPaginationAsync(PaginationRequest request)
        {
            var cacheKey = CacheNameConstant.CategoruPagination + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out CategoryPaginationResponse response))
            {
                return response;
            };

            response = new CategoryPaginationResponse();

            var categories = _repoWrapper.Category.FindByCondition(x => x.IsInactive == request.IsInactive);

            if (request.SearchKeyword != null)
            {
                var searchString = request.SearchKeyword.ToLower();
                categories = categories.Where(x => x.Name.ToLower().Contains(searchString)
                                                || x.Description.ToLower().Contains(searchString));

            }
            response.Count = await categories.CountAsync();

            var result = await categories.OrderByDescending(x => x.UpdatedAt)
                                         .Pagination(request)
                                         .ToListAsync();

            response.Data = _mapper.Map<List<CategoryResponse>>(result);
            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        #endregion
    }
}
