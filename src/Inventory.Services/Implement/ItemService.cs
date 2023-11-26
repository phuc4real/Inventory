using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Category;
using Inventory.Service.DTO.Item;
using Inventory.Service.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Inventory.Service.Implement
{
    public class ItemService : BaseService, IItemService
    {
        #region Ctor & Field

        private readonly IUserService _userService;

        public ItemService(
            IRepoWrapper repoWrapper,
            IMapper mapper,
            ICommonService commonService,
            IUserService userService,
            IRedisCacheService cacheService,
            IEmailService emailService
            )
        : base(repoWrapper, mapper, commonService, cacheService, emailService)
        {
            _userService = userService;
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
                response.Message = new("Error", "Category not found!");
                return response;
            }

            var dupCodeItem = await _repoWrapper.Item.FirstOrDefaultAsync(x => !x.IsInactive && x.Code == request.Code);

            if (dupCodeItem != null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Error", "Duplicated item code!");
                return response;
            }

            Item item = _mapper.Map<Item>(request);

            var result = ItemValidation.Validate(item);

            if (!result.Message.IsNullOrEmpty())
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = result;
                return response;
            }

            await _repoWrapper.Item.AddAsync(item);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<ItemResponse>(item);
            response.Message = new ResultMessage("Success", "Item create successfully!");
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

            var result = await (from item in _repoWrapper.Item.FindByCondition(x => x.Id == request.Id.Value)
                                join cate in _repoWrapper.Category.FindAll()
                                on item.CategoryId equals cate.Id

                                join u1 in _repoWrapper.User
                                on item.CreatedBy equals u1.UserName
                                join u2 in _repoWrapper.User
                                on item.UpdatedBy equals u2.UserName

                                select new
                                {
                                    item,
                                    CreatedBy = u1.FirstName + " " + u1.LastName,
                                    updatedBy = u2.FirstName + " " + u2.LastName,
                                    cate
                                }).FirstOrDefaultAsync();

            result.item.CreatedBy = result.CreatedBy;
            result.item.UpdatedBy = result.updatedBy;
            result.item.Category = result.cate;

            if (result == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Error", "Item not found!");
                return response;
            }

            response.Data = _mapper.Map<ItemResponse>(result.item);

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
                response.Message = new("Error", "Item not found!");
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
                response.Message = new("Error", "Item not exists!");
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
                response.Message = new("Error", "Item not exists!");
                return response;
            }

            _repoWrapper.Item.Remove(item);
            await _repoWrapper.SaveAsync();

            response.Message = new("Success", "Item deactive succesfully!");
            await _cacheService.RemoveCacheTreeAsync(CacheNameConstant.ItemPagination);
            await _cacheService.RemoveCacheAsync(CacheNameConstant.Item + item.Id);
            return response;
        }

        public async Task<ItemHolderListResponse> GetItemHolderAsync(PaginationRequest request)
        {
            var response = new ItemHolderListResponse();
            var userName = request.GetUserContext();
            var permission = await _userService.CheckRoleOfUser(userName);
            var isAdminOrSuperAdmin = permission.IsAdmin || permission.IsSuperAdmin;

            var query = (from export in _repoWrapper.Export.FindByCondition(x => isAdminOrSuperAdmin
                                                                              ? x.IsInactive == request.IsInactive
                                                                              : x.CreatedBy == userName
                                                                              && x.IsInactive == request.IsInactive)
                         join status in _repoWrapper.Status.FindByCondition(x => x.Name == StatusConstant.Done)
                         on export.StatusId equals status.Id

                         join entry in _repoWrapper.ExportEntry.FindAll()
                         on export.Id equals entry.ExportId
                         join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                         on entry.ItemId equals item.Id

                         join user in _repoWrapper.User
                         on export.ExportFor equals user.UserName

                         select new ItemHolderResponse
                         {
                             ItemId = item.Id,
                             ItemCode = item.Code,
                             ItemName = item.Name,
                             ItemImageUrl = item.ImageUrl,
                             CategoryId = item.CategoryId,
                             CategoryName = item.Category.Name,
                             ExportId = export.Id,
                             UserName = user.UserName,
                             Email = user.Email,
                             FullName = user.FirstName + " " + user.LastName
                         }); ;

            if (request.SearchKeyword != null)
            {
                var searchString = request.SearchKeyword.ToLower();
                query = query.Where(x => x.ItemCode.ToLower().Contains(searchString)
                                      || x.ItemName.ToLower().Contains(searchString)
                                      || x.CategoryName.ToLower().Contains(searchString)
                                      || x.UserName.ToLower().Contains(searchString)
                                      || x.Email.ToLower().Contains(searchString)
                                      || x.FullName.ToLower().Contains(searchString));
            }

            response.Count = await query.CountAsync();
            response.Data = await query.OrderByDescending(x => x.ItemId)
                                    .Pagination(request)
                                    .ToListAsync();
            return response;
        }

        #endregion
    }
}
