using AutoMapper;
using Inventory.Core.Common;
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

        public ItemService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
            : base(repoWrapper, mapper, cacheService)
        {
        }

        #endregion

        #region Method

        public async Task<ItemObjectResponse> CreateAsync(ItemUpdateRequest request)
        {
            ItemObjectResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            request.Id = null;

            Item item = _mapper.Map<Item>(request);

            await _repoWrapper.Item.AddAsync(item);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<ItemResponse>(item);

            await _cacheService.RemoveCacheTreeAsync("item");
            return response;
        }

        public async Task<ItemPaginationResponse> GetPaginationAsync(PaginationRequest request)
        {
            var cacheKey = "item" + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ItemPaginationResponse response))
            {
                return response;
            };

            response = new ItemPaginationResponse();

            var items = _repoWrapper.Item.FindByCondition(x => x.IsInactive == request.IsInactive)
                                         .Include(x => x.Category);

            response.Count = await items.CountAsync();

            List<Item> result = new();
            if (request.SearchKeyword != null)
            {
                var searchString = request.SearchKeyword.ToLower();
                result = await items.Where(x => x.Name.ToLower().Contains(searchString)
                                             || x.Category.Name.ToLower().Contains(searchString)
                                             || x.Code.ToLower().Contains(searchString))
                                    .Pagination(request)
                                    .ToListAsync();
            }
            else
            {
                result = await items.Pagination(request)
                                    .ToListAsync();
            }

            response.Data = _mapper.Map<List<ItemResponse>>(result);
            await _cacheService.SetCacheAsync(cacheKey, response);

            return response;
        }

        public async Task<ItemObjectResponse> GetByIdAsync(ItemRequest request)
        {
            var cacheKey = "item?id=" + request.Id!.Value;
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

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<ItemObjectResponse> UpdateAsync(ItemUpdateRequest request)
        {
            ItemObjectResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var item = await _repoWrapper.Item.FindByCondition(x => x.Id == request.Id)
                                                    .FirstOrDefaultAsync();

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

            await _cacheService.RemoveCacheTreeAsync("item");
            return response;
        }

        public async Task<BaseResponse> DeactiveAsync(ItemRequest request)
        {
            BaseResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var item = await _repoWrapper.Item.FindByCondition(x => x.Id == request.Id)
                                                      .FirstOrDefaultAsync();

            if (item == null || item.IsInactive)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Item", "Item not exists!");
                return response;
            }

            _repoWrapper.Item.Remove(item);
            await _repoWrapper.SaveAsync();

            response.Message = new("Item", "Item deleted!");
            await _cacheService.RemoveCacheTreeAsync("item");
            return response;
        }

        #endregion
    }
}
