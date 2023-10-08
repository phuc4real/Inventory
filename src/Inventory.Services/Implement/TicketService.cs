using AutoMapper;
using Azure.Core;
using Inventory.Core.Common;
using Inventory.Core.Const;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Ticket;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Implement
{
    public class TicketService : BaseService, ITicketService
    {

        #region Ctor & Field

        private readonly IUserService _userServices;

        public TicketService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService, IUserService userServices)
            : base(repoWrapper, mapper, cacheService)
        {
            _userServices = userServices;
        }

        #endregion

        #region Method

        public async Task<TicketPageResponse> GetPaginationAsync(PaginationRequest request)
        {
            var cacheKey = "ticket" + request.GetUserContext() + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out TicketPageResponse response))
            {
                return response;
            };

            var listTicket = await (from ticket in _repoWrapper.Ticket.FindByCondition(x => x.IsInactive == request.IsInactive)
                                    join record in _repoWrapper.TicketRecord.FindAll()
                                    on ticket.Id equals record.TicketId
                                    join status in _repoWrapper.Status.FindAll()
                                    on record.StatusId equals status.Id
                                    join type in _repoWrapper.TicketType.FindAll()
                                    on record.TicketTypeId equals type.Id
                                    select new TicketResponse
                                    {
                                        TicketId = ticket.Id,
                                        RecordId = record.Id,
                                        Description = record.Description,
                                        Status = status.Name,
                                        TicketType = type.Name,
                                        Title = record.Title,
                                        IsClosed = ticket.CloseDate != null,
                                        ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                                        CreatedAt = record.CreatedAt,
                                        CreatedBy = record.CreatedBy,
                                        UpdatedAt = record.UpdatedAt,
                                        UpdatedBy = record.UpdatedBy
                                    })
                                   .OrderBy(x => x.UpdatedAt)
                                   .ToListAsync();

            var tickets = listTicket.GroupBy(x => x.TicketId).Select(x => x.FirstOrDefault()).ToList();

            var userId = request.GetUserContext();
            var permission = await _userServices.CheckRoleOfUser(userId);

            if (!permission.IsAdmin && !permission.IsSuperAdmin)
            {
                tickets = tickets.Where(x => x.CreatedBy == userId)
                                 .ToList();
            }

            response.Count = tickets.Count;

            var result = await tickets.AsQueryable().Pagination(request).ToListAsync();

            response.Data = _mapper.Map<List<TicketResponse>>(result);

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<TicketObjectResponse> GetByIdAsync(TicketRequest request)
        {
            var cacheKey = "ticket" + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out TicketObjectResponse response))
            {
                return response;
            };

            response = new TicketObjectResponse();

            var result = await (from ticket in _repoWrapper.Ticket.FindAll()
                                join record in _repoWrapper.TicketRecord.FindByCondition(x => x.Id == request.RecordId)
                                on ticket.Id equals record.TicketId
                                join status in _repoWrapper.Status.FindAll()
                                on record.StatusId equals status.Id
                                join type in _repoWrapper.TicketType.FindAll()
                                on record.TicketTypeId equals type.Id
                                select new TicketResponse
                                {
                                    TicketId = ticket.Id,
                                    RecordId = record.Id,
                                    Description = record.Description,
                                    Status = status.Name,
                                    TicketType = type.Name,
                                    Title = record.Title,
                                    IsClosed = ticket.CloseDate != null,
                                    ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                                    CreatedAt = record.CreatedAt,
                                    CreatedBy = record.CreatedBy,
                                    UpdatedAt = record.UpdatedAt,
                                    UpdatedBy = record.UpdatedBy
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");
                return response;
            }

            response.Data = result;

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<TicketObjectResponse> CreateAsync(TicketUpdateResquest request)
        {
            TicketObjectResponse response = new();

            //Add Ticket 
            var ticket = new Ticket()
            {
                CloseDate = null
            };

            await _repoWrapper.Ticket.AddAsync(ticket);

            //Add Ticket record
            var status = await _repoWrapper.Status.FindByCondition(x => x.Name == StatusConstant.Pending)
                                                    .FirstOrDefaultAsync();
            var record = new TicketRecord()
            {
                TicketId = ticket.Id,
                Description = request.Description,
                Title = request.Title,
                TicketTypeId = request.TicketTypeId,
                StatusId = status!.Id,
            };

            await _repoWrapper.TicketRecord.AddAsync(record);

            //Add Ticket Entry
            var entries = _mapper.Map<List<TicketEntry>>(request.TicketEntries);

            entries.ForEach(x => x.RecordId = record.Id);

            await _repoWrapper.TicketEntry.AddRangeAsync(entries);

            await _repoWrapper.SaveAsync();

            var type = await _repoWrapper.TicketType.FindByCondition(x => x.Id == record.TicketTypeId)
                                                    .FirstOrDefaultAsync();

            response.Data = new TicketResponse()
            {
                TicketId = ticket.Id,
                RecordId = record.Id,
                Description = record.Description,
                Status = status.Name,
                TicketType = type.Name,
                Title = record.Title,
                IsClosed = ticket.CloseDate != null,
                ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                CreatedAt = record.CreatedAt,
                CreatedBy = record.CreatedBy,
                UpdatedAt = record.UpdatedAt,
                UpdatedBy = record.UpdatedBy
            };

            await _cacheService.RemoveCacheTreeAsync("ticket");
            return response;
        }

        public async Task<BaseResponse> CancelAsync(TicketRequest request)
        {
            BaseResponse response = new();

            var record = await _repoWrapper.TicketRecord.FindByCondition(x => x.Id == request.RecordId)
                                                        .FirstOrDefaultAsync();

            if (record == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");

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
                _repoWrapper.TicketRecord.Update(record);

                //Set order complete date
                var ticket = await _repoWrapper.Ticket.FindByCondition(x => x.Id == record.TicketId)
                                                    .FirstOrDefaultAsync();

                ticket.CloseDate = DateTime.UtcNow;
                _repoWrapper.Ticket.Update(ticket);

                await _repoWrapper.SaveAsync();

                response.Message = new("Ticket", "Ticket has been canceled");

                await _cacheService.RemoveCacheTreeAsync("ticket");
                return response;
            }
            response.Message = new("Ticket", "Cannot cancel ticket");

            return response;
        }

        public async Task<BaseResponse> UpdateStatusAsync(TicketRequest request)
        {
            BaseResponse response = new();

            var record = await _repoWrapper.TicketRecord.FindByCondition(x => x.Id == request.RecordId)
                                                       .FirstOrDefaultAsync();

            if (record == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");

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
                var ticket = await _repoWrapper.Ticket.FindByCondition(x => x.Id == record.TicketId)
                                                    .FirstOrDefaultAsync();

                ticket.CloseDate = DateTime.UtcNow;
                _repoWrapper.Ticket.Update(ticket);
            }

            _repoWrapper.TicketRecord.Update(record);

            await _repoWrapper.SaveAsync();

            response.Message = new("Ticket", "Update status successfully");

            await _cacheService.RemoveCacheTreeAsync("ticket");
            return response;
        }

        public async Task<ChartDataResponse> GetTicketChart()
        {
            var cacheKey = "ticket.chartdata";
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ChartDataResponse response))
            {
                return response;
            };

            response = new ChartDataResponse();

            var last12Month = DateTime.UtcNow.AddMonths(-11);
            last12Month = last12Month.AddDays(1 - last12Month.Day);

            var query = await _repoWrapper.Ticket.FindByCondition(x => !x.IsInactive)
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
