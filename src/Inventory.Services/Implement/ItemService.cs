using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Item;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Implement
{
    public class ItemService : BaseService, IItemService
    {
        #region Ctor & Field

        public ItemService(
            IRepoWrapper repoWrapper,
            IMapper mapper,
            ICommonService commonService,
            IRedisCacheService cacheService,
            IEmailService emailService
            )
        : base(repoWrapper, mapper, commonService, cacheService, emailService)
        {
        }

        #endregion

        #region Method

        public async Task<ItemObjectResponse> CreateAsync(ItemUpdateRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            ItemObjectResponse response = new();

            request.Id = null;

            var cate = await _repoWrapper.Category.FirstOrDefaultAsync(x => !x.IsInactive && x.Id == request.CategoryId);

            if (cate == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Category", "Category not found!");
                return response;
            }

            var dupCodeItem = await _repoWrapper.Item.FirstOrDefaultAsync(x => !x.IsInactive && x.Code == request.Code);

            if (dupCodeItem != null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Item", "Duplicated item code!");
                return response;
            }

            Item item = _mapper.Map<Item>(request);

            await _repoWrapper.Item.AddAsync(item);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<ItemResponse>(item);
            response.Message = new ResultMessage("Created", "Item create successfully!");
            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.ItemPagination);
            return response;
        }

        public async Task<ItemPaginationResponse> GetPaginationAsync(PaginationRequest request)
        {
            var cacheKey = CacheNameConstant.ItemPagination + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ItemPaginationResponse response))
            {
                return response;
            };

            response = new ItemPaginationResponse();

            var items = _repoWrapper.Item.FindByCondition(x => x.IsInactive == request.IsInactive);

            if (request.SearchKeyword != null)
            {
                var searchString = request.SearchKeyword.ToLower();
                items = items.Where(x => x.Name.ToLower().Contains(searchString)
                                      || x.Category.Name.ToLower().Contains(searchString)
                                      || x.Code.ToLower().Contains(searchString));
            }


            response.Count = await items.CountAsync();
            var result = await items.OrderByDescending(x => x.UpdatedAt)
                                    .Include(x => x.Category)
                                    .Pagination(request)
                                    .ToListAsync();



            response.Data = _mapper.Map<List<ItemResponse>>(result);
            await _cacheService.SetCacheAsync(cacheKey, response);

            return response;
        }

        public async Task<ItemObjectResponse> GetByIdAsync(ItemRequest request)
        {
            var cacheKey = CacheNameConstant.Item + request.Id!.Value;
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ItemObjectResponse response))
            {
                return response;
            };

            response = new ItemObjectResponse();

            var item = await _repoWrapper.Item.FindByCondition(x => x.Id == request.Id.Value)
                                              .Include(x => x.Category)
                                              .FirstOrDefaultAsync();
            if (item == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Item", "Not found!");
                return response;
            }

            response.Data = _mapper.Map<ItemResponse>(item);

            (response.Data.CreatedBy, response.Data.UpdatedBy) = await _commonService.GetAuditUserData(item.CreatedBy, item.UpdatedBy);

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<ItemCompactObjectResponse> GetByIdCompactAsync(ItemRequest request)
        {
            var cacheKey = CacheNameConstant.ItemCompact + request.Id!.Value;
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ItemCompactObjectResponse response))
            {
                return response;
            };

            response = new ItemCompactObjectResponse();

            var item = await _repoWrapper.Item.FindByCondition(x => x.Id == request.Id.Value)
                                              .Include(x => x.Category)
                                              .FirstOrDefaultAsync();
            if (item == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Item", "Not found!");
                return response;
            }

            response.Data = _mapper.Map<ItemCompactResponse>(item);

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<ItemObjectResponse> UpdateAsync(ItemUpdateRequest request)
        {
            ItemObjectResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var item = await _repoWrapper.Item.FirstOrDefaultAsync(x => !x.IsInactive && x.Id == request.Id);

            if (item == null || item.IsInactive)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Item", "Item not exists!");
                return response;
            }

            item.Code = request.Code;
            item.Name = request.Name;
            item.Description = request.Description;
            item.ImageUrl = request.ImageUrl;
            item.CategoryId = request.CategoryId;

            _repoWrapper.Item.Update(item);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<ItemResponse>(item);

            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.ItemPagination);
            await _cacheService.RemoveCacheAsync(CacheNameConstant.Item + item.Id);
            return response;
        }

        public async Task<BaseResponse> DeactiveAsync(ItemRequest request)
        {
            BaseResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var item = await _repoWrapper.Item.FirstOrDefaultAsync(x => !x.IsInactive && x.Id == request.Id);

            if (item == null || item.IsInactive)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Item", "Item not exists!");
                return response;
            }

            _repoWrapper.Item.Remove(item);
            await _repoWrapper.SaveAsync();

            response.Message = new("Item", "Item deactive succesfully!");
            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.ItemPagination);
            await _cacheService.RemoveCacheAsync(CacheNameConstant.Item + item.Id);
            return response;
        }

        #endregion
    }
}
