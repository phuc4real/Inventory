using AutoMapper;
using AutoMapper.QueryableExtensions;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Comment;
using Inventory.Service.DTO.Email;
using Inventory.Service.DTO.Export;
using Inventory.Service.DTO.Item;
using Inventory.Service.DTO.Order;
using Inventory.Service.DTO.Ticket;
using Inventory.Service.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;

namespace Inventory.Service.Implement
{
    public class TicketService : BaseService, ITicketService
    {

        #region Ctor & Field

        private readonly IExportService _exportService;
        private readonly IUserService _userService;

        public TicketService(IRepoWrapper repoWrapper,
                             IMapper mapper,
                             ICommonService commonService,
                             IRedisCacheService cacheService,
                             IEmailService emailService,
                             IExportService exportService,
                             IUserService userService)
        : base(repoWrapper, mapper, commonService, cacheService, emailService)
        {
            _exportService = exportService;
            _userService = userService;
        }

        #endregion

        #region Method

        public async Task<TicketPageResponse> GetPaginationAsync(PaginationRequest request)
        {
            var response = new TicketPageResponse()
            {
                Data = new List<TicketResponse>()
            };

            var userName = request.GetUserContext();
            var permission = await _userService.CheckRoleOfUser(userName);
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
                                         Status = status.Description,
                                         TicketType = type.Description,
                                         Title = record.Title,
                                         IsClosed = ticket.CloseDate != null,
                                         ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                                         CreatedAt = record.CreatedAt,
                                         CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                                         UpdatedAt = record.UpdatedAt,
                                         UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                                     })
                               .Distinct()
                               .OrderByDescending(x => x.UpdatedAt)
                               .ToListAsync();

            var listTicket = ticketQuery.GroupBy(x => x.TicketId)
                                        .Select(x => x.OrderByDescending(x => x.UpdatedAt)
                                                      .FirstOrDefault());

            response.Count = listTicket.Count();
            var result = listTicket.AsQueryable()
                                   .Pagination(request)
                                   .ProjectTo<TicketResponse>(_mapper.ConfigurationProvider)
                                   .ToList();

            var reviewTicket = result.Where(x => x.Status == "In Review").ToList();
            response.Data.AddRange(reviewTicket);

            var remainTicket = result.Where(x => x.Status != "In Review").OrderBy(x => x.Status).ToList();
            response.Data.AddRange(remainTicket);

            return response;
        }

        public async Task<TicketObjectResponse> GetByIdAsync(TicketRequest request)
        {
            var response = new TicketObjectResponse();

            var userName = request.GetUserContext();
            var permission = await _userService.CheckRoleOfUser(userName);
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

                                join entry in _repoWrapper.TicketEntry.FindAll()
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
                                    Status = status.Description,
                                    TicketType = type.Description,
                                    TicketTypeId = type.Id,
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
                response.AddError("Ticket not found!");
                return response;
            }

            result.Comment = await _commonService.GetComment(result.RecordId, true);

            response.Data = result;

            response.History = await GetHistoryByTicketId(result.TicketId);

            return response;
        }

        public async Task<TicketObjectResponse> CreateOrUpdateAsync(TicketUpdateResquest request)
        {
            var userName = request.GetUserContext();
            _repoWrapper.SetUserContext(userName);
            TicketObjectResponse response = new();

            var type = await _repoWrapper.TicketType.FindAll().ToListAsync();
            var status = await _commonService.GetStatusCollections();

            var err = TicketValidation.Validate(request);

            if (!err.Message.IsNullOrEmpty())
            {
                response.AddError(err);
                return response;
            }

            //Case create new Ticket
            if (request.RecordId == 0)
            {
                var ticket = new Ticket()
                {
                    CloseDate = null
                };

                await _repoWrapper.Ticket.AddAsync(ticket);
                await _repoWrapper.SaveAsync();

                var record = new TicketRecord()
                {
                    TicketId = ticket.Id,
                    Description = request.Description,
                    Title = request.Title,
                    TicketTypeId = request.TicketTypeId,
                    StatusId = status.ReviewId,
                };

                await _repoWrapper.TicketRecord.AddAsync(record);
                await _repoWrapper.SaveAsync();

                var entries = _mapper.Map<List<TicketEntry>>(request.TicketEntries);

                entries.ForEach(x => x.RecordId = record.Id);

                await _repoWrapper.TicketEntry.AddRangeAsync(entries);
                await _repoWrapper.SaveAsync();

                var user = (await _userService.GetByUserNameAsync(ticket.CreatedBy)).Data;

                response.Data = new TicketResponse()
                {
                    TicketId = ticket.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Status = status.Data.FirstOrDefault(x => x.Id == record.StatusId)?.Description,
                    TicketType = type.FirstOrDefault(x => x.Id == record.TicketTypeId)?.Description,
                    Title = record.Title,
                    IsClosed = ticket.CloseDate != null,
                    ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                    CreatedAt = record.CreatedAt,
                    CreatedBy = user.FirstName + " " + user.LastName,
                    UpdatedAt = record.UpdatedAt,
                    UpdatedBy = record.UpdatedBy
                };

                try
                {
                    SendNotification(response.Data, $"New ticket #{ticket.Id} has been created!");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                return response;
            }
            //Case update ticket
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

                if (ticketAndRecord == null)
                {
                    response.AddError("Ticket not found!");
                    return response;
                }

                var ticket = ticketAndRecord.Ticket;
                var oldRecord = ticketAndRecord.Record;

                var recordCount = await _repoWrapper.TicketRecord.FindByCondition(x => x.TicketId == ticket.Id).CountAsync();

                if (recordCount >= 3)
                {
                    response.AddError("Cannot edit ticket!, A ticket can only edit 3 times");
                    return response;
                }

                if (status.CannotEdit.Contains(oldRecord.StatusId))
                {
                    response.AddError("Cannot edit ticket!");
                    return response;
                }

                //Add Ticket record
                var record = new TicketRecord()
                {
                    TicketId = ticket.Id,
                    Description = request.Description,
                    Title = request.Title,
                    TicketTypeId = request.TicketTypeId,
                    StatusId = status.ReviewId,
                };

                oldRecord.StatusId = status.CloseId;

                _repoWrapper.TicketRecord.Update(oldRecord);
                await _repoWrapper.TicketRecord.AddAsync(record);
                await _repoWrapper.SaveAsync();

                //Add Ticket Entry
                var entries = _mapper.Map<List<TicketEntry>>(request.TicketEntries);

                entries.ForEach(x => x.RecordId = record.Id);

                await _repoWrapper.TicketEntry.AddRangeAsync(entries);

                await _repoWrapper.SaveAsync();

                var user = (await _userService.GetByUserNameAsync(ticket.CreatedBy)).Data;

                response.Data = new TicketResponse()
                {
                    TicketId = ticket.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Status = status.Data.FirstOrDefault(x => x.Id == record.StatusId)?.Description,
                    TicketType = type.FirstOrDefault(x => x.Id == record.TicketTypeId)?.Description,
                    Title = record.Title,
                    IsClosed = ticket.CloseDate != null,
                    ClosedDate = ticket.CloseDate.GetValueOrDefault(),
                    CreatedAt = record.CreatedAt,
                    CreatedBy = user.FirstName + " " + user.LastName,
                    UpdatedAt = record.UpdatedAt,
                    UpdatedBy = record.UpdatedBy
                };

                try
                {
                    SendNotification(response.Data, $"Ticket #{ticket.Id} has been changed!");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }

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
                response.AddError("Ticket not found!");

                return response;
            };

            var record = await _repoWrapper.TicketRecord.FindByCondition(x => x.TicketId == ticket.Id)
                                                        .OrderByDescending(x => x.UpdatedAt)
                                                        .FirstOrDefaultAsync();

            var status = await _commonService.GetStatusCollections();

            if (status.CanCancelTicket.Contains(record.StatusId))
            {
                if (record.StatusId == status.ProcessingId)
                {
                    var export = await _repoWrapper.Export.FindByCondition(x => !x.IsInactive && x.TicketId == ticket.Id)
                                                .OrderByDescending(x => x.UpdatedAt)
                                                .FirstOrDefaultAsync();

                    export.StatusId = status.CancelId;
                    _repoWrapper.Export.Update(export);
                }

                record.StatusId = status.CancelId;
                _repoWrapper.TicketRecord.Update(record);

                ticket.CloseDate = DateTime.UtcNow;
                _repoWrapper.Ticket.Update(ticket);

                await _repoWrapper.SaveAsync();

                response.AddMessage("Ticket has been canceled");
                return response;
            }
            else
            {
                response.AddError("Cannot cancel ticket");
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
                response.AddError("Ticket not found!");

                return response;
            };

            var record = await _repoWrapper.TicketRecord.FindByCondition(x => x.TicketId == ticket.Id)
                                                        .OrderByDescending(x => x.UpdatedAt)
                                                        .FirstOrDefaultAsync();

            if (record == null)
            {
                response.AddError("Ticket not found!");

                return response;
            }

            var status = await _commonService.GetStatusCollections();

            if (record.StatusId == status.PendingId)
            {
                var newExport = await _exportService.CreateFromTicketAsync(new ExportCreateRequest
                {
                    RecordId = record.Id,
                    TicketId = ticket.Id,
                });

                if (newExport != null && newExport.StatusCode == ResponseCode.BadRequest)
                {
                    response.AddMessage(newExport.Message);
                    return response;
                }

                record.StatusId = status.ProcessingId;
            }
            else if (record.StatusId == status.ProcessingId)
            {

                var export = await _repoWrapper.Export.FindByCondition(x => !x.IsInactive && x.TicketId == ticket.Id)
                                                .OrderByDescending(x => x.UpdatedAt)
                                                .FirstOrDefaultAsync();

                if (export?.StatusId == status.DoneId)
                {
                    record.StatusId = status.DoneId;

                    ticket.CloseDate = DateTime.UtcNow;
                    _repoWrapper.Ticket.Update(ticket);
                }
                else
                {
                    response.AddError("Export for this ticket not done yet!");

                    return response;
                }
            }
            else
            {
                response.AddError("Cannot change status!");

                return response;
            }

            _repoWrapper.TicketRecord.Update(record);
            await _repoWrapper.SaveAsync();

            response.AddMessage("Update status successfully");

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
                                 select new TicketEntryResponse
                                 {
                                     Id = entry.Id,
                                     RecordId = record.Id,
                                     Item = _mapper.Map<ItemCompactResponse>(item),
                                     Quantity = entry.Quantity,
                                     Note = entry.Note,
                                 }).ToListAsync();
            if (entries.Any())
            {
                response.Data = entries;
            }
            else
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.AddError("Ticket not found!");
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
            var status = await _commonService.GetStatusCollections();

            var result = await (from t in _repoWrapper.Ticket.FindByCondition(x => !x.IsInactive)
                                join r in _repoWrapper.TicketRecord.FindByCondition(x => !x.IsInactive)
                                on t.Id equals r.TicketId
                                join s in _repoWrapper.Status.FindByCondition(x => status.SummaryId.Contains(x.Id))
                                on r.StatusId equals s.Id
                                group r by r.StatusId into gr
                                select new
                                {
                                    StatusId = gr.Key,
                                    Count = gr.Count()
                                }
                                ).ToListAsync();

            response.Data = new TicketSummaryResponse
            {
                Review = result.FirstOrDefault(x => x.StatusId == status.ReviewId)?.Count ?? 0,
                Pending = result.FirstOrDefault(x => x.StatusId == status.PendingId)?.Count ?? 0,
                Processing = result.FirstOrDefault(x => x.StatusId == status.ProcessingId)?.Count ?? 0,
                Done = result.FirstOrDefault(x => x.StatusId == status.DoneId)?.Count ?? 0,
            };

            return response;
        }

        public async Task<BaseResponse> ApprovalTicketAsync(int recordId, CreateCommentRequest request)
        {
            _repoWrapper.SetUserContext(request.GetUserContext());
            var response = new BaseResponse();

            var status = await _commonService.GetStatusCollections();
            var ticketAndRecord = await (from t in _repoWrapper.Ticket.FindByCondition(x => !x.IsInactive)
                                         join r in _repoWrapper.TicketRecord.FindByCondition(x => !x.IsInactive && x.Id == request.RecordId)
                                         on t.Id equals r.TicketId
                                         select new
                                         {
                                             Ticket = t,
                                             Record = r,
                                         }).FirstOrDefaultAsync();


            if (ticketAndRecord == null)
            {
                response.AddError("Ticket not found!");

                return response;
            }

            var ticket = ticketAndRecord.Ticket;
            var record = ticketAndRecord.Record;

            if (record.StatusId != status.ReviewId)
            {
                response.AddError("Cannot approval ticket!");

                return response;
            }

            var comment = await _commonService.AddNewComment(request);

            if (request.IsReject)
            {
                record.StatusId = status.RejectId;
            }
            else
            {
                //Update ticket status
                record.StatusId = status.PendingId;
            }

            try
            {
                var user = await _repoWrapper.User.FirstOrDefaultAsync(x => x.UserName == record.CreatedBy);

                SendNotification(new TicketResponse()
                {
                    TicketId = ticket.Id,
                    RecordId = record.Id,
                    Description = record.Description,
                    Title = record.Title,
                    CreatedAt = record.CreatedAt,
                    CreatedBy = user.FirstName + " " + user.LastName,
                },
                "Your ticket #" + ticket.Id + " has been " + (request.IsReject ? "rejected!" : "approval!"),
                user.Email);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            _repoWrapper.TicketRecord.Update(record);
            _repoWrapper.Ticket.Update(ticket);
            await _repoWrapper.SaveAsync();

            response.AddMessage("Thank for approve the ticket!");

            return response;
        }

        #endregion

        #region Private 

        public async Task<List<RecordHistoryResponse>> GetHistoryByTicketId(int ticketId)
        {
            var history = await (from record in _repoWrapper.TicketRecord.FindByCondition(x => x.TicketId == ticketId)
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
                history.ForEach(x => x.Number = i++);
                return history;
            }
            else
                return new List<RecordHistoryResponse> { };
        }

        private async void SendNotification(TicketResponse ticket, string subject, string? toEmail = null)
        {
            var request = new NotificationEmailRequest()
            {
                Subject = subject,
                Body = new EmailBodyData()
                {
                    InfoId = ticket.TicketId,
                    RecordId = ticket.RecordId,
                    IsTicket = true,
                    TicketType = ticket.TicketType,
                    Title = ticket.Title,
                    InfoCreatedBy = ticket.CreatedBy,
                    InfoCreatedAt = ticket.CreatedAt,
                    Description = ticket.Description
                }
            };

            if (!toEmail.IsNullOrEmpty())
            {
                request.SendTo(ticket.CreatedBy, toEmail);
            }

            await _emailService.SendNotification(request);
        }

        #endregion
    }
}
