using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using Inventory.Service.DTO.Item;
using Inventory.Service.DTO.Order;
using Inventory.Service.DTO.Ticket;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Implement
{
    public class OrderService : BaseService, IOrderService
    {
        #region Ctor & Field

        public OrderService(
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

        public async Task<OrderPageResponse> GetPaginationAsync(PaginationRequest request)
        {
            var response = new OrderPageResponse();

            var search = request.SearchKeyword != null ? request.SearchKeyword?.ToLower() : "";
            var orderQuery = await (from order in _repoWrapper.Order.FindByCondition(x => x.IsInactive == request.IsInactive)
                                    join record in _repoWrapper.OrderRecord.FindAll()
                                    on order.Id equals record.OrderId
                                    join s1 in _repoWrapper.Status.FindAll()
                                    on record.StatusId equals s1.Id into left3
                                    from status in left3.DefaultIfEmpty()


                                    join entry in _repoWrapper.OrderEntry.FindAll()
                                    on record.Id equals entry.RecordId
                                    join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                    on entry.ItemId equals item.Id

                                    join u1 in _repoWrapper.User
                                    on record.CreatedBy equals u1.UserName into left1
                                    from createdBy in left1.DefaultIfEmpty()
                                    join u2 in _repoWrapper.User
                                    on record.UpdatedBy equals u2.UserName into left2
                                    from updatedBy in left2.DefaultIfEmpty()
                                    where item.Name.Contains(search) || order.Id.ToString().Contains(search)
                                    select new OrderResponse
                                    {
                                        OrderId = order.Id,
                                        RecordId = record.Id,
                                        Description = record.Description,
                                        Status = status.Description,
                                        IsCompleted = order.CompleteDate != null,
                                        CompletedDate = order.CompleteDate.GetValueOrDefault(),
                                        CreatedAt = record.CreatedAt,
                                        CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                                        UpdatedAt = record.UpdatedAt,
                                        UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                                    })
                              .Distinct()
                              .ToListAsync();

            var listOrder = orderQuery.GroupBy(x => x.OrderId)
                                      .Select(x => x.OrderByDescending(x => x.UpdatedAt)
                                                    .FirstOrDefault());
            response.Count = listOrder.Count();
            response.Data = listOrder.AsQueryable()
                                     .Pagination(request)
                                     .ProjectTo<OrderResponse>(_mapper.ConfigurationProvider)
                                     .ToList();

            return response;
        }

        public async Task<OrderObjectResponse> CreateOrUpdateAsync(OrderUpdateRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            OrderObjectResponse response = new();

            var status = await _commonService.GetStatusCollections();

            if (request.RecordId == 0)
            {
                //Add Order 
                var order = new Model.Entity.Order()
                {
                    CompleteDate = null
                };

                await _repoWrapper.Order.AddAsync(order);
                await _repoWrapper.SaveAsync();

                //Add Order record
                var record = new OrderRecord()
                {
                    OrderId = order.Id,
                    Description = request.Description,
                    StatusId = status.ReviewId,
                };

                await _repoWrapper.OrderRecord.AddAsync(record);
                await _repoWrapper.SaveAsync();

                //Add Order Entry
                var entries = _mapper.Map<List<OrderEntry>>(request.OrderEntries);

                entries.ForEach(x => x.RecordId = record.Id);

                await _repoWrapper.OrderEntry.AddRangeAsync(entries);

                await _repoWrapper.SaveAsync();

                //_emailService.Send()
                response.Data = new OrderResponse()
                {
                    OrderId = order.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Status = status.Data.FirstOrDefault(x => x.Id == record.StatusId).Description,
                    IsCompleted = order.CompleteDate != null,
                    CompletedDate = order.CompleteDate.GetValueOrDefault(),
                    CreatedAt = record.CreatedAt,
                    CreatedBy = record.CreatedBy,
                    UpdatedAt = record.UpdatedAt,
                    UpdatedBy = record.UpdatedBy
                };
                return response;
            }
            else
            {
                var orderAndRecord = await (from o in _repoWrapper.Order.FindByCondition(x => !x.IsInactive)
                                            join r in _repoWrapper.OrderRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                            on o.Id equals r.OrderId
                                            select new
                                            {
                                                Order = o,
                                                Record = r,
                                            }
                                   ).FirstOrDefaultAsync();

                var order = orderAndRecord.Order;
                var oldRecord = orderAndRecord.Record;

                if (status.CannotEdit.Contains(oldRecord.StatusId))
                {
                    response.StatusCode = ResponseCode.BadRequest;
                    response.Message = new("Error", "Cannot edit order!");
                    return response;
                }

                var record = new OrderRecord()
                {
                    OrderId = order.Id,
                    Description = request.Description,
                    StatusId = status.ReviewId,
                };

                oldRecord.StatusId = status.CloseId;

                _repoWrapper.OrderRecord.Update(oldRecord);
                await _repoWrapper.OrderRecord.AddAsync(record);
                await _repoWrapper.SaveAsync();

                //Add Order Entry
                var entries = _mapper.Map<List<OrderEntry>>(request.OrderEntries);

                entries.ForEach(x => x.RecordId = record.Id);

                await _repoWrapper.OrderEntry.AddRangeAsync(entries);

                await _repoWrapper.SaveAsync();

                //_emailService.Send()
                response.Data = new OrderResponse()
                {
                    OrderId = order.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Status = status.Data.FirstOrDefault(x => x.Id == record.StatusId)?.Description,
                    IsCompleted = order.CompleteDate != null,
                    CompletedDate = order.CompleteDate.GetValueOrDefault(),
                    CreatedAt = record.CreatedAt,
                    CreatedBy = record.CreatedBy,
                    UpdatedAt = record.UpdatedAt,
                    UpdatedBy = record.UpdatedBy
                };
                return response;
            }
        }

        public async Task<OrderObjectResponse> GetByIdAsync(OrderRequest request)
        {
            var response = new OrderObjectResponse();

            var result = await (from record in _repoWrapper.OrderRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                join order in _repoWrapper.Order.FindByCondition(x => !x.IsInactive)
                                on record.OrderId equals order.Id
                                join status in _repoWrapper.Status.FindAll()
                                on record.StatusId equals status.Id

                                join entry in _repoWrapper.OrderEntry.FindAll()
                                on record.Id equals entry.RecordId
                                join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                on entry.ItemId equals item.Id

                                join u1 in _repoWrapper.User
                                on record.CreatedBy equals u1.UserName into left1
                                from createdBy in left1.DefaultIfEmpty()
                                join u2 in _repoWrapper.User
                                on record.UpdatedBy equals u2.UserName into left2
                                from updatedBy in left2.DefaultIfEmpty()

                                select new OrderResponse
                                {
                                    OrderId = order.Id,
                                    RecordId = record.Id,
                                    Description = record.Description,
                                    Status = status.Description,
                                    IsCompleted = order.CompleteDate != null,
                                    CompletedDate = order.CompleteDate.GetValueOrDefault(),
                                    CreatedAt = record.CreatedAt,
                                    CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                                    UpdatedAt = record.UpdatedAt,
                                    UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");
                return response;
            }

            response.Data = result;
            response.History = await GetHistoryByOrderId(result.OrderId);

            return response;
        }

        public async Task<BaseResponse> UpdateOrderStatusAsync(OrderRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            BaseResponse response = new();

            var order = await _repoWrapper.Order.FindByCondition(x => !x.IsInactive && x.Id == request.OrderId)
                                                .FirstOrDefaultAsync();
            if (order == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");

                return response;
            };

            var record = await _repoWrapper.OrderRecord.FindByCondition(x => x.OrderId == order.Id)
                                                        .OrderByDescending(x => x.UpdatedAt)
                                                        .FirstOrDefaultAsync();

            if (record == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");

                return response;
            }

            var status = await _commonService.GetStatusCollections();

            if (record.StatusId == status.PendingId)
            {
                record.StatusId = status.ProcessingId;
            }
            else if (record.StatusId == status.ProcessingId)
            {
                record.StatusId = status.DoneId;

                //Set order complete date
                order.CompleteDate = DateTime.UtcNow;
                _repoWrapper.Order.Update(order);
            }
            else
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Cannot change status!");

                return response;
            }

            _repoWrapper.OrderRecord.Update(record);
            await _repoWrapper.SaveAsync();

            response.Message = new("Order", "Update status successfully");

            return response;
        }

        public async Task<BaseResponse> CancelOrderAsync(OrderRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            BaseResponse response = new();

            var order = await _repoWrapper.Order.FindByCondition(x => !x.IsInactive && x.Id == request.OrderId)
                                                .FirstOrDefaultAsync();
            if (order == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Order", "Order not found!");

                return response;
            };

            var record = await _repoWrapper.OrderRecord.FindByCondition(x => x.OrderId == order.Id)
                                                        .OrderByDescending(x => x.UpdatedAt)
                                                        .FirstOrDefaultAsync();

            var status = await _commonService.GetStatusCollections();

            if (status.CanEdit.Contains(record.StatusId))
            {
                record.StatusId = status.CancelId;
                _repoWrapper.OrderRecord.Update(record);
                _repoWrapper.Order.Update(order);
                await _repoWrapper.SaveAsync();

                response.Message = new("Order", "Order has been canceled");

                return response;
            }
            response.StatusCode = ResponseCode.BadRequest;
            response.Message = new("Order", "Cannot cancel order");

            return response;
        }

        public async Task<ChartDataResponse> GetOrderChartAsync()
        {
            var response = new ChartDataResponse();

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

            return response;
        }

        public async Task<OrderEntryListResponse> GetOrderEntries(OrderRequest request)
        {
            var response = new OrderEntryListResponse();

            var entries = await (from record in _repoWrapper.OrderRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                 join entry in _repoWrapper.OrderEntry.FindAll()
                                 on record.Id equals entry.RecordId
                                 join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                 on entry.ItemId equals item.Id
                                 select new
                                 {
                                     Description = record.Description,
                                     Entry = new OrderEntryResponse
                                     {
                                         Id = entry.Id,
                                         RecordId = record.Id,
                                         Item = _mapper.Map<ItemCompactResponse>(item),
                                         Quantity = entry.Quantity,
                                         MaxPrice = entry.MaxPrice,
                                         MinPrice = entry.MinPrice,
                                         Note = entry.Note,
                                     }
                                 }).ToListAsync();
            if (entries.Any())
            {
                response.Data = entries.Select(x => x.Entry).ToList();
                response.Description = entries.Select(x => x.Description).FirstOrDefault();
            }
            else
            {
                response.Message = new("Order", "Order has been canceled");
            }
            return response;
        }


        public async Task<BaseResponse> ApprovalOrderAsync(int recordId, CreateCommentRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            var response = new BaseResponse();

            var status = await _commonService.GetStatusCollections();
            var orderAndRecord = await (from o in _repoWrapper.Order.FindByCondition(x => !x.IsInactive)
                                        join r in _repoWrapper.OrderRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                        on o.Id equals r.OrderId
                                        select new
                                        {
                                            Order = o,
                                            Record = r,
                                        }).FirstOrDefaultAsync();

            if (orderAndRecord == null)
            {
                response.Message = new("Error", "Order not found!");
                response.StatusCode = ResponseCode.BadRequest;

                return response;
            }

            var order = orderAndRecord.Order;
            var record = orderAndRecord.Record;

            if (record.StatusId != status.ReviewId)
            {
                response.Message = new("Error", "Cannot approval order!");
                response.StatusCode = ResponseCode.BadRequest;

                return response;
            }

            var comment = await _commonService.AddNewComment(request);

            if (request.IsReject)
            {
                record.StatusId = status.RejectId;
            }
            else
            {
                record.StatusId = status.PendingId;
            }
            _repoWrapper.OrderRecord.Update(record);
            _repoWrapper.Order.Update(order);
            await _repoWrapper.SaveAsync();

            response.Message = new("Ticket", "Thank for approve the order!");

            return response;
        }

        #endregion

        #region Private

        private async Task<List<RecordHistoryResponse>> GetHistoryByOrderId(int orderId)
        {
            var history = await (from record in _repoWrapper.OrderRecord.FindByCondition(x => x.OrderId == orderId)
                                 join created in _repoWrapper.User
                                 on record.CreatedBy equals created.UserName
                                 select new RecordHistoryResponse
                                 {
                                     Number = 0,
                                     RecordId = record.Id,
                                     CreatedAt = record.CreatedAt,
                                     CreatedBy = created.FirstName + " " + created.LastName,
                                 })
                                 .ToListAsync();

            if (history.Count > 0)
            {
                int i = 1;
                foreach (var item in history)
                {
                    item.Number = i++;
                }
                return history;
            }
            else
                return new List<RecordHistoryResponse> { };
        }

        #endregion
    }
}
