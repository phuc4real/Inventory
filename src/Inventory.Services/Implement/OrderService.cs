using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Const;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Order;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Implement
{
    public class OrderService : BaseService, IOrderService
    {
        #region Ctor & Field

        public OrderService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
            : base(repoWrapper, mapper, cacheService)
        {
        }

        #endregion


        #region Method

        public async Task<OrderPageResponse> GetPaginationAsync(PaginationRequest request)
        {
            var cacheKey = "order" + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out OrderPageResponse response))
            {
                return response;
            };

            response = new OrderPageResponse();

            var listOrder = await (from order in _repoWrapper.Order.FindByCondition(x => x.IsInactive == request.IsInactive)
                                   join record in _repoWrapper.OrderRecord.FindAll()
                                   on order.Id equals record.OrderId
                                   join status in _repoWrapper.Status.FindAll()
                                   on record.StatusId equals status.Id
                                   select new OrderResponse
                                   {
                                       OrderId = order.Id,
                                       RecordId = record.Id,
                                       Description = record.Description,
                                       Status = status.Name,
                                       IsCompleted = order.CompleteDate != null,
                                       CompletedDate = order.CompleteDate.GetValueOrDefault(),
                                       CreatedAt = record.CreatedAt,
                                       CreatedBy = record.CreatedBy,
                                       UpdatedAt = record.UpdatedAt,
                                       UpdatedBy = record.UpdatedBy
                                   })
                                   .OrderBy(x => x.UpdatedAt)
                                   .ToListAsync();

            var orders = listOrder.GroupBy(x => x.OrderId).Select(x => x.FirstOrDefault()).ToList();

            response.Count = orders.Count;

            var result = await orders.AsQueryable().Pagination(request).ToListAsync();

            response.Data = _mapper.Map<List<OrderResponse>>(result);
            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<OrderObjectResponse> CreateAsync(OrderUpdateRequest request)
        {
            OrderObjectResponse response = new();

            //Add Order 
            var order = new Order()
            {
                CompleteDate = null
            };

            await _repoWrapper.Order.AddAsync(order);

            //Add Order record
            var status = await _repoWrapper.Status.FindByCondition(x => x.Name == StatusConstant.Pending)
                                                    .FirstOrDefaultAsync();
            var record = new OrderRecord()
            {
                OrderId = order.Id,
                Description = request.Description,
                StatusId = status!.Id,
            };

            await _repoWrapper.OrderRecord.AddAsync(record);

            //Add Order Entry
            var entries = _mapper.Map<List<OrderEntry>>(request.OrderEntries);

            entries.ForEach(x => x.RecordId = record.Id);

            await _repoWrapper.OrderEntry.AddRangeAsync(entries);

            await _repoWrapper.SaveAsync();

            response.Data = new OrderResponse()
            {
                OrderId = order.Id,
                RecordId = record.Id,
                Description = record.Description,
                Status = status.Name,
                IsCompleted = order.CompleteDate != null,
                CompletedDate = order.CompleteDate.GetValueOrDefault(),
                CreatedAt = record.CreatedAt,
                CreatedBy = record.CreatedBy,
                UpdatedAt = record.UpdatedAt,
                UpdatedBy = record.UpdatedBy
            };

            await _cacheService.RemoveCacheTreeAsync("order");
            return response;
        }

        public async Task<OrderObjectResponse> GetByIdAsync(OrderRequest request)
        {
            var cacheKey = "order" + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out OrderObjectResponse response))
            {
                return response;
            };

            response = new OrderObjectResponse();

            var result = await (from record in _repoWrapper.OrderRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                join order in _repoWrapper.Order.FindByCondition(x => !x.IsInactive)
                                on record.OrderId equals order.Id
                                join status in _repoWrapper.Status.FindAll()
                                on record.StatusId equals status.Id
                                select new OrderResponse
                                {
                                    OrderId = order.Id,
                                    RecordId = record.Id,
                                    Description = record.Description,
                                    Status = status.Name,
                                    IsCompleted = order.CompleteDate != null,
                                    CompletedDate = order.CompleteDate.GetValueOrDefault(),
                                    CreatedAt = record.CreatedAt,
                                    CreatedBy = record.CreatedBy,
                                    UpdatedAt = record.UpdatedAt,
                                    UpdatedBy = record.UpdatedBy
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");
                return response;
            }

            response.Data = result;

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<BaseResponse> UpdateOrderStatusAsync(OrderRequest request)
        {
            BaseResponse response = new();

            var record = await _repoWrapper.OrderRecord.FindByCondition(x => x.Id == request.RecordId)
                                                       .FirstOrDefaultAsync();

            if (record == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");

                return response;
            }

            var listStatus = await _repoWrapper.Status.FindByCondition(x => x.Name == StatusConstant.Pending
                                                                         || x.Name == StatusConstant.Processing
                                                                         || x.Name == StatusConstant.Done)
                                                      .ToListAsync();

            var pending = listStatus.Where(x => x.Name == StatusConstant.Pending)
                                    .FirstOrDefault();

            var processing = listStatus.Where(x => x.Name == StatusConstant.Processing)
                                       .FirstOrDefault();

            var done = listStatus.Where(x => x.Name == StatusConstant.Done)
                                 .FirstOrDefault();

            if (record.StatusId == pending.Id)
            {
                record.StatusId = processing.Id;
            }

            if (record.StatusId == processing.Id)
            {
                record.StatusId = done.Id;

                //Set order complete date
                var order = await _repoWrapper.Order.FindByCondition(x => x.Id == record.OrderId)
                                                    .FirstOrDefaultAsync();

                order.CompleteDate = DateTime.UtcNow;
                _repoWrapper.Order.Update(order);
            }

            _repoWrapper.OrderRecord.Update(record);

            await _repoWrapper.SaveAsync();

            response.Message = new("Order", "Update status successfully");

            await _cacheService.RemoveCacheTreeAsync("order");
            return response;
        }

        public async Task<BaseResponse> CancelOrderAsync(OrderRequest request)
        {
            BaseResponse response = new();

            var record = await _repoWrapper.OrderRecord.FindByCondition(x => x.Id == request.RecordId)
                                                       .FirstOrDefaultAsync();

            if (record == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");

                return response;
            }

            var listStatus = await _repoWrapper.Status.FindByCondition(x => x.Name == StatusConstant.Done
                                                                         || x.Name == StatusConstant.Cancel)
                                                      .ToListAsync();

            var cancel = listStatus.Where(x => x.Name == StatusConstant.Cancel)
                                   .FirstOrDefault();

            var done = listStatus.Where(x => x.Name == StatusConstant.Done)
                                 .FirstOrDefault();

            if (record.StatusId != done.Id && record.StatusId != cancel.Id)
            {
                record.StatusId = cancel.Id;
                _repoWrapper.OrderRecord.Update(record);
                await _repoWrapper.SaveAsync();

                response.Message = new("Order", "Order has been canceled");

                await _cacheService.RemoveCacheTreeAsync("order");
                return response;
            }
            response.Message = new("Order", "Cannot cancel order");

            return response;
        }

        public async Task<ChartDataResponse> GetOrderChartAsync()
        {
            var cacheKey = "order.chartdata";
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ChartDataResponse response))
            {
                return response;
            };

            response = new ChartDataResponse();

            var last12Month = DateTime.UtcNow.AddMonths(-11);
            last12Month = last12Month.AddDays(1 - last12Month.Day);

            var query = await _repoWrapper.Order.FindByCondition(x => !x.IsInactive)
                .Where(x => x.CreatedAt > last12Month)
                .GroupBy(x => new { x.CreatedAt.Month, x.CreatedAt.Year })
                .ToListAsync();

            response.Data = query.Select(x => new ChartData
            {
                Month = x.Key.Month + "/" + x.Key.Year,
                Value = x.Count()
            }).ToList();

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        #endregion
    }
}
