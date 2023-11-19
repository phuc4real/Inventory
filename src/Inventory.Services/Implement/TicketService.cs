using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Item;
using Inventory.Service.DTO.Order;
using Inventory.Service.DTO.Ticket;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Inventory.Service.Implement
{
    public class TicketService : BaseService, ITicketService
    {

        #region Ctor & Field

        private readonly IUserService _userServices;

        public TicketService(IRepoWrapper repoWrapper,
                             IMapper mapper,
                             ICommonService commonService,
                             IRedisCacheService cacheService,
                             IEmailService emailService,
                             IUserService userServices)
        : base(repoWrapper, mapper, commonService, cacheService, emailService)
        {
            _userServices = userServices;
        }

        #endregion

        #region Method

        public async Task<TicketPageResponse> GetPaginationAsync(PaginationRequest request)
        {
            var response = new TicketPageResponse();

            var userName = request.GetUserContext();
            var permission = await _userServices.CheckRoleOfUser(userName);
            var isAdminOrSuperAdmin = permission.IsAdmin || permission.IsSuperAdmin;

            var search = request.SearchKeyword != null ? request.SearchKeyword?.ToLower() : "";
            var ticketQuery = await (from ticket in _repoWrapper.Ticket.FindByCondition(x => isAdminOrSuperAdmin
                                                                                        ? x.IsInactive == request.IsInactive
                                                                                        : x.CreatedBy == userName
                                                                                        && x.IsInactive == request.IsInactive)
                                     join record in _repoWrapper.TicketRecord.FindAll()
                                     on ticket.Id equals record.TicketId
                                     join s1 in _repoWrapper.Status.FindAll()
                                     on record.StatusId equals s1.Id into left1
                                     from status in left1.DefaultIfEmpty()
                                     join t1 in _repoWrapper.TicketType.FindAll()
                                     on record.TicketTypeId equals t1.Id into left2
                                     from type in left2.DefaultIfEmpty()

                                     join entry in _repoWrapper.TicketEntry.FindAll()
                                     on record.Id equals entry.RecordId
                                     join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                     on entry.ItemId equals item.Id

                                     join u1 in _repoWrapper.User
                                     on record.CreatedBy equals u1.UserName into left3
                                     from createdBy in left3.DefaultIfEmpty()
                                     join u2 in _repoWrapper.User
                                     on record.UpdatedBy equals u2.UserName into left4
                                     from updatedBy in left4.DefaultIfEmpty()
                                     where item.Name.ToLower().Contains(search)
                                        || ticket.Id.ToString().Contains(search)
                                        || record.Title.ToLower().Contains(search)
                                        || record.Description.ToLower().Contains(search)
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
                                         CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                                         UpdatedAt = record.UpdatedAt,
                                         UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                                     })
                               .Distinct()
                               .ToListAsync();

            var listTicket = ticketQuery.GroupBy(x => x.TicketId)
                                        .Select(x => x.OrderByDescending(x => x.UpdatedAt)
                                                      .FirstOrDefault());

            response.Count = listTicket.Count();
            response.Data = listTicket.AsQueryable()
                                  .Pagination(request)
                                  .ProjectTo<TicketResponse>(_mapper.ConfigurationProvider)
                                  .ToList();

            return response;
        }

        public async Task<TicketObjectResponse> GetByIdAsync(TicketRequest request)
        {
            var response = new TicketObjectResponse();

            var userName = request.GetUserContext();
            var permission = await _userServices.CheckRoleOfUser(userName);
            var isAdminOrSuperAdmin = permission.IsAdmin || permission.IsSuperAdmin;

            var result = await (from ticket in _repoWrapper.Ticket.FindByCondition(x => isAdminOrSuperAdmin
                                                                                        ? x.IsInactive == false
                                                                                        : x.CreatedBy == userName
                                                                                        && x.IsInactive == false)
                                join record in _repoWrapper.TicketRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                on ticket.Id equals record.TicketId
                                join status in _repoWrapper.Status.FindAll()
                                on record.StatusId equals status.Id
                                join type in _repoWrapper.TicketType.FindAll()
                                on record.TicketTypeId equals type.Id

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
                                    CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                                    UpdatedAt = record.UpdatedAt,
                                    UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");
                return response;
            }

            response.Data = result;

            return response;
        }

        public async Task<TicketObjectResponse> CreateOrUpdateAsync(TicketUpdateResquest request)
        {
            var userName = request.GetUserContext();
            _repoWrapper.SetUserContext(userName);
            TicketObjectResponse response = new();

            var type = await _repoWrapper.TicketType.FindAll().ToListAsync();
            var status = await _repoWrapper.Status.FindAll().ToListAsync();

            if (request.RecordId == 0)
            {
                //Add Ticket 
                var ticket = new Ticket()
                {
                    CloseDate = null
                };

                await _repoWrapper.Ticket.AddAsync(ticket);
                await _repoWrapper.SaveAsync();

                //Add Ticket record
                var statusPending = status.FirstOrDefault(x => x.Name == StatusConstant.Pending);
                var record = new TicketRecord()
                {
                    TicketId = ticket.Id,
                    Description = request.Description,
                    Title = request.Title,
                    TicketTypeId = request.TicketTypeId,
                    StatusId = statusPending.Id,
                };

                await _repoWrapper.TicketRecord.AddAsync(record);
                await _repoWrapper.SaveAsync();

                //Add Ticket Entry
                var entries = _mapper.Map<List<TicketEntry>>(request.TicketEntries);

                entries.ForEach(x => x.RecordId = record.Id);

                await _repoWrapper.TicketEntry.AddRangeAsync(entries);

                await _repoWrapper.SaveAsync();

                response.Data = new TicketResponse()
                {
                    TicketId = ticket.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Status = status.FirstOrDefault(x => x.Id == record.StatusId)?.Name,
                    TicketType = type.FirstOrDefault(x => x.Id == record.TicketTypeId)?.Name,
                    Title = record.Title,
                    IsClosed = ticket.CloseDate != null,
                    ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                    CreatedAt = record.CreatedAt,
                    CreatedBy = record.CreatedBy,
                    UpdatedAt = record.UpdatedAt,
                    UpdatedBy = record.UpdatedBy
                };

                return response;
            }
            else
            {
                var ticketAndRecord = await (from t in _repoWrapper.Ticket.FindByCondition(x => !x.IsInactive && x.CreatedBy == userName)
                                             join r in _repoWrapper.TicketRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                             on t.Id equals r.TicketId
                                             select new
                                             {
                                                 Ticket = t,
                                                 Record = r,
                                             }).FirstOrDefaultAsync();

                var ticket = ticketAndRecord.Ticket;
                var oldRecord = ticketAndRecord.Record;

                var statusCannotEdit = status.Where(x => x.Name != StatusConstant.Pending
                                                        && x.Name != StatusConstant.Processing)
                                            .Select(x => x.Id);
                var statusPending = status.FirstOrDefault(x => x.Name == StatusConstant.Pending);
                var statusClosed = status.FirstOrDefault(x => x.Name == StatusConstant.Close);

                //Add Ticket record
                var record = new TicketRecord()
                {
                    TicketId = ticket.Id,
                    Description = request.Description,
                    Title = request.Title,
                    TicketTypeId = request.TicketTypeId,
                    StatusId = statusPending.Id,
                };

                oldRecord.StatusId = statusClosed.Id;

                _repoWrapper.TicketRecord.Update(oldRecord);
                await _repoWrapper.TicketRecord.AddAsync(record);
                await _repoWrapper.SaveAsync();

                //Add Ticket Entry
                var entries = _mapper.Map<List<TicketEntry>>(request.TicketEntries);

                entries.ForEach(x => x.RecordId = record.Id);

                await _repoWrapper.TicketEntry.AddRangeAsync(entries);

                await _repoWrapper.SaveAsync();

                response.Data = new TicketResponse()
                {
                    TicketId = ticket.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Status = status.FirstOrDefault(x => x.Id == record.StatusId)?.Name,
                    TicketType = type.FirstOrDefault(x => x.Id == record.TicketTypeId)?.Name,
                    Title = record.Title,
                    IsClosed = ticket.CloseDate != null,
                    ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                    CreatedAt = record.CreatedAt,
                    CreatedBy = record.CreatedBy,
                    UpdatedAt = record.UpdatedAt,
                    UpdatedBy = record.UpdatedBy
                };

                return response;
            }
        }

        public async Task<BaseResponse> CancelAsync(TicketRequest request)
        {
            BaseResponse response = new();

            var ticket = await _repoWrapper.Ticket.FindByCondition(x => !x.IsInactive && x.Id == request.TicketId)
                                               .FirstOrDefaultAsync();
            if (ticket == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");

                return response;
            };

            var record = await _repoWrapper.TicketRecord.FindByCondition(x => x.TicketId == ticket.Id)
                                                        .OrderByDescending(x => x.UpdatedAt)
                                                        .FirstOrDefaultAsync();

            var listStatus = await _repoWrapper.Status.FindByCondition(x => x.Name == StatusConstant.Done
                                                                         || x.Name == StatusConstant.Cancel)
                                                      .ToListAsync();

            var statusCancel = listStatus.FirstOrDefault(x => x.Name == StatusConstant.Cancel);
            var statusDone = listStatus.FirstOrDefault(x => x.Name == StatusConstant.Done);

            if (record.StatusId != statusDone.Id && record.StatusId != statusCancel.Id)
            {
                record.StatusId = statusCancel.Id;
                _repoWrapper.TicketRecord.Update(record);

                ticket.CloseDate = DateTime.UtcNow;
                _repoWrapper.Ticket.Update(ticket);

                await _repoWrapper.SaveAsync();

                response.Message = new("Ticket", "Ticket has been canceled");
                return response;
            }
            else
            {
                response.Message = new("Ticket", "Cannot cancel ticket");
                return response;
            }

        }

        public async Task<BaseResponse> UpdateStatusAsync(TicketRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            BaseResponse response = new();

            var ticket = await _repoWrapper.Ticket.FirstOrDefaultAsync(x => !x.IsInactive && x.Id == request.TicketId);
            if (ticket == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");

                return response;
            };

            var record = await _repoWrapper.TicketRecord.FindByCondition(x => x.TicketId == ticket.Id)
                                                        .OrderByDescending(x => x.UpdatedAt)
                                                        .FirstOrDefaultAsync();

            if (record == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Ticket not found!");

                return response;
            }

            var status = await _repoWrapper.Status.FindAll().ToListAsync();

            var statusPending = status.FirstOrDefault(x => x.Name == StatusConstant.Pending);
            var statusProcessing = status.FirstOrDefault(x => x.Name == StatusConstant.Processing);
            var statusDone = status.FirstOrDefault(x => x.Name == StatusConstant.Done);

            if (record.StatusId == statusPending.Id)
            {
                record.StatusId = statusProcessing.Id;
            }
            else if (record.StatusId == statusProcessing.Id)
            {
                record.StatusId = statusDone.Id;

                ticket.CloseDate = DateTime.UtcNow;
                _repoWrapper.Ticket.Update(ticket);
            }
            else
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Ticket", "Cannot change status!");

                return response;
            }

            _repoWrapper.TicketRecord.Update(record);
            await _repoWrapper.SaveAsync();

            response.Message = new("Ticket", "Update status successfully");

            return response;
        }

        public async Task<TicketEntryList> GetTicketEntries(TicketRequest request)
        {
            var response = new TicketEntryList();

            var entries = await (from record in _repoWrapper.TicketRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                 join entry in _repoWrapper.TicketEntry.FindAll()
                                 on record.Id equals entry.RecordId
                                 join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                 on entry.ItemId equals item.Id
                                 select new
                                 {
                                     Description = record.Description,
                                     Entry = new TicketEntryResponse
                                     {
                                         Id = entry.Id,
                                         RecordId = record.Id,
                                         Item = _mapper.Map<ItemCompactResponse>(item),
                                         Quantity = entry.Quantity,
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

        public async Task<TicketTypeList> GetTicketType()
        {
            var result = await _repoWrapper.TicketType.FindAll()
                                                      .ProjectTo<TicketTypeResponse>(_mapper.ConfigurationProvider)
                                                      .ToListAsync();
            return new TicketTypeList
            {
                Data = result
            };
        }

        public async Task<TicketSummaryObjectResponse> GetTicketSummary()
        {
            var response = new TicketSummaryObjectResponse();
            var status = await _repoWrapper.Status.FindAll().ToListAsync();

            var statusPending = status.FirstOrDefault(x => x.Name == StatusConstant.Pending);
            var statusProcessing = status.FirstOrDefault(x => x.Name == StatusConstant.Processing);
            var statusRejected = status.FirstOrDefault(x => x.Name == StatusConstant.Rejected);
            var statusCompleted = status.FirstOrDefault(x => x.Name == StatusConstant.Close);

            var result = await (
                from t in _repoWrapper.Ticket.FindByCondition(x => !x.IsInactive)
                join r in _repoWrapper.TicketRecord.FindByCondition(x => !x.IsInactive)
                on t.Id equals r.TicketId
                select new TicketSummaryResponse
                {
                    PendingTicket = r.StatusId == statusPending.Id ? 1 : 0,
                    ProcessingTicket = r.StatusId == statusProcessing.Id ? 1 : 0,
                    RejectTicket = r.StatusId == statusRejected.Id ? 1 : 0,
                    CompletedTicket = r.StatusId == statusCompleted.Id ? 1 : 0,
                }).ToListAsync();

            response.Data = new TicketSummaryResponse
            {
                PendingTicket = result.Sum(x => x.PendingTicket),
                ProcessingTicket = result.Sum(x => x.ProcessingTicket),
                RejectTicket = result.Sum(x => x.RejectTicket),
                CompletedTicket = result.Sum(x => x.CompletedTicket)
            };

            return response;
        }

        #endregion
    }
}
