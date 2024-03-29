﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Export;
using Inventory.Service.DTO.Item;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Implement
{
    public class ExportService : BaseService, IExportService
    {
        #region Ctor & Field
        public ExportService(
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

        public async Task<ExportPaginationResponse> GetPaginationAsync(PaginationRequest request)
        {
            var response = new ExportPaginationResponse() { Data = new List<ExportResponse>() };

            var search = request.SearchKeyword != null ? request.SearchKeyword?.ToLower() : "";
            var exports = (from export in _repoWrapper.Export.FindByCondition(x => x.IsInactive == request.IsInactive)
                           join status in _repoWrapper.Status.FindAll()
                           on export.StatusId equals status.Id

                           join entry in _repoWrapper.ExportEntry.FindAll()
                           on export.Id equals entry.ExportId
                           join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                           on entry.ItemId equals item.Id

                           join u1 in _repoWrapper.User
                           on export.CreatedBy equals u1.UserName into left1
                           from createdBy in left1.DefaultIfEmpty()
                           join u2 in _repoWrapper.User
                           on export.UpdatedBy equals u2.UserName into left2
                           from updatedBy in left2.DefaultIfEmpty()
                           join u3 in _repoWrapper.User
                            on export.ExportFor equals u3.UserName into left3
                           from ExportFor in left3.DefaultIfEmpty()

                           where item.Name.ToLower().Contains(search)
                                         || export.Id.ToString().Contains(search)
                                         || export.Description.ToLower().Contains(search)
                           select new ExportResponse
                           {
                               Id = export.Id,
                               Description = export.Description,
                               Status = status.Description,
                               ExportFor = ExportFor.FirstName + " " + ExportFor.LastName,
                               CreatedAt = export.CreatedAt,
                               CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                               UpdatedAt = export.UpdatedAt,
                               UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                           }).Distinct();

            response.Count = await exports.CountAsync();
            var result = await exports.Pagination(request)
                                      .ToListAsync();

            var review = result.Where(x => x.Status == "In Review").ToList();
            response.Data.AddRange(review);

            var remain = result.Where(x => x.Status != "In Review").OrderBy(x => x.Status).ToList();
            response.Data.AddRange(remain);

            return response;
        }

        public async Task<ExportObjectResponse> GetByIdAsync(ExportRequest request)
        {
            var response = new ExportObjectResponse();

            var result = await (from export in _repoWrapper.Export.FindByCondition(x => x.Id == request.Id)
                                join status in _repoWrapper.Status.FindAll()
                                on export.StatusId equals status.Id

                                join u1 in _repoWrapper.User
                                on export.CreatedBy equals u1.UserName into left1
                                from createdBy in left1.DefaultIfEmpty()
                                join u2 in _repoWrapper.User
                                on export.UpdatedBy equals u2.UserName into left2
                                from updatedBy in left2.DefaultIfEmpty()
                                join u3 in _repoWrapper.User
                                on export.ExportFor equals u3.UserName into left3
                                from ExportFor in left3.DefaultIfEmpty()

                                select new ExportResponse
                                {
                                    Id = export.Id,
                                    Description = export.Description,
                                    Status = status.Description,
                                    ExportFor = ExportFor.FirstName + " " + ExportFor.LastName,
                                    CreatedAt = export.CreatedAt,
                                    CreatedBy = createdBy.FirstName + " " + createdBy.LastName,
                                    UpdatedAt = export.UpdatedAt,
                                    UpdatedBy = updatedBy.FirstName + " " + updatedBy.LastName
                                }).FirstOrDefaultAsync();

            if (result == null)
            {
                response.AddError("Export Not found!");
                return response;
            }

            response.Data = result;

            return response;
        }

        public async Task<ChartDataResponse> GetChartDataAsync()
        {
            var response = new ChartDataResponse();

            var last12Month = DateTime.UtcNow.AddMonths(-11);
            last12Month = last12Month.AddDays(1 - last12Month.Day);

            var query = await _repoWrapper.Export.FindByCondition(x => !x.IsInactive)
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

        public async Task<BaseResponse> UpdateExportStatusAsync(ExportRequest request)
        {
            BaseResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var status = await _commonService.GetStatusCollections();

            var export = await _repoWrapper.Export
                .FindByCondition(x => x.Id == request.Id
                                   && status.CanEdit.Contains(x.StatusId))
                .FirstOrDefaultAsync();

            if (export == null)
            {
                response.AddError("Export not found");
                return response;
            }

            if (export.StatusId == status.PendingId)
            {
                //TODO: check have enough unit of item
                var entries = await (from entry in _repoWrapper.ExportEntry.FindByCondition(x => x.ExportId == export.Id)
                                     join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                     on entry.ItemId equals item.Id
                                     select new
                                     {
                                         entry,
                                         item,
                                         IsEnoughUnit = (item.Unit - entry.Quantity) > 0
                                     }).ToListAsync();

                var errors = "";
                foreach (var e in entries)
                {
                    if (!e.IsEnoughUnit)
                    {
                        errors += $"Item {e.item.Code} not have enough unit. ";
                    }
                }

                if (errors != "")
                {
                    response.AddError(errors);
                    return response;
                }

                export.StatusId = status.ProcessingId;
            }
            else if (export.StatusId == status.ProcessingId)
            {
                var data = await (from entry in _repoWrapper.ExportEntry.FindByCondition(x => x.ExportId == export.Id)
                                  join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                  on entry.ItemId equals item.Id
                                  select new
                                  {
                                      entry,
                                      item,
                                  }).ToListAsync();

                foreach (var e in data)
                {
                    e.item.Unit -= e.entry.Quantity;
                    e.item.UseUnit += e.entry.Quantity;
                }

                var items = data.Select(x => x.item).ToList();
                _repoWrapper.Item.UpdateRange(items);
                await _cacheService.RemoveCacheByListIdAsync(items.Select(x => x.Id));

                export.StatusId = status.DoneId;
            }

            _repoWrapper.Export.Update(export);
            await _repoWrapper.SaveAsync();

            return response;
        }

        public async Task<ExportObjectResponse> CreateFromTicketAsync(ExportCreateRequest request)
        {
            var response = new ExportObjectResponse();

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

            var (ticket, record) = (ticketAndRecord.Ticket, ticketAndRecord.Record);

            var entries = await _repoWrapper.TicketEntry.FindByCondition(x => x.RecordId == record.Id)
                                                        .ProjectTo<ExportEntry>(_mapper.ConfigurationProvider)
                                                        .ToListAsync();
            var export = new Export
            {
                TicketId = ticket.Id,
                Description = record.Description,
                ExportFor = ticket.CreatedBy,
                StatusId = status.PendingId
            };

            await _repoWrapper.Export.AddAsync(export);
            await _repoWrapper.SaveAsync();

            entries.ForEach(x => x.ExportId = export.Id);
            await _repoWrapper.ExportEntry.AddRangeAsync(entries);
            await _repoWrapper.SaveAsync();

            response.Data = _mapper.Map<ExportResponse>(export);
            response.AddMessage("Create export successfully!");

            return response;
        }

        public async Task<ExportEntryListResponse> GetEntriesAsync(ExportRequest request)
        {
            var response = new ExportEntryListResponse();

            var entries = await (from record in _repoWrapper.Export.FindByCondition(x => !x.IsInactive && x.Id == request.Id)
                                 join entry in _repoWrapper.ExportEntry.FindAll()
                                 on record.Id equals entry.ExportId
                                 join item in _repoWrapper.Item.FindByCondition(x => !x.IsInactive)
                                 on entry.ItemId equals item.Id
                                 select new ExportEntryResponse
                                 {
                                     Id = entry.Id,
                                     ExportId = entry.ExportId,
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
                response.AddError("Export not found!");
            }
            return response;
        }

        #endregion
    }
}
