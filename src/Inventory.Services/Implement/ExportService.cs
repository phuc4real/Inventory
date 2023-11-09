using AutoMapper;
using Azure.Core;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.Common;
using Inventory.Service.DTO.Category;
using Inventory.Service.DTO.Export;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Service.Implement
{
    public class ExportService : BaseService, IExportService
    {
        #region Ctor & Field
        public ExportService(IRepoWrapper repoWrapper, IMapper mapper, IRedisCacheService cacheService)
            : base(repoWrapper, mapper, cacheService)
        {
        }

        #endregion

        #region Method

        public async Task<ExportPaginationResponse> GetPaginationAsync(PaginationRequest request)
        {
            var cacheKey = "export" + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ExportPaginationResponse response))
            {
                return response;
            };

            response = new ExportPaginationResponse();

            var exports = _repoWrapper.Export.FindByCondition(x => x.IsInactive == request.IsInactive);

            response.Count = await exports.CountAsync();

            var result = await exports.Pagination(request).ToListAsync();

            response.Data = _mapper.Map<List<ExportResponse>>(result);

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<ExportObjectResponse> GetByIdAsync(ExportRequest request)
        {
            var cacheKey = "export?id=" + request.GetQueryString();
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ExportObjectResponse response))
            {
                return response;
            };

            response = new ExportObjectResponse();

            var export = await _repoWrapper.Export.FindByCondition(x => !x.IsInactive && x.Id == request.Id)
                                                    .FirstOrDefaultAsync();
            if (export == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Export", "Not found!");
                return response;
            }

            response.Data = _mapper.Map<ExportResponse>(export);

            await _cacheService.SetCacheAsync(cacheKey, response);

            return response;
        }

        public async Task<ChartDataResponse> GetChartDataAsync()
        {
            var cacheKey = "export.chartdata";
            //try get from redis cache
            if (_cacheService.TryGetCacheAsync(cacheKey, out ChartDataResponse response))
            {
                return response;
            };
            response = new ChartDataResponse();

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

            await _cacheService.SetCacheAsync(cacheKey, response);
            return response;
        }

        public async Task<BaseResponse> UpdateExportStatusAsync(ExportRequest request)
        {
            BaseResponse response = new();

            _repoWrapper.SetUserContext(request.GetUserContext());

            var status = await _repoWrapper.Status
                .FindByCondition(x => x.Name == StatusConstant.Pending
                                   || x.Name == StatusConstant.Processing
                                   || x.Name == StatusConstant.Done)
                .ToListAsync();

            var export = await _repoWrapper.Export
                .FindByCondition(x => x.Id == request.Id
                                   && status.Where(x => x.Name == StatusConstant.Pending
                                                     || x.Name == StatusConstant.Processing)
                                   .Select(x => x.Id)
                                   .Contains(x.StatusId))
                .FirstOrDefaultAsync();

            if (export == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("Export", "Export not found");
                return response;
            }

            var statusPending = status.Where(x => x.Name == StatusConstant.Pending).Select(x => x.Id).FirstOrDefault();
            var statusProcessing = status.Where(x => x.Name == StatusConstant.Done).Select(x => x.Id).FirstOrDefault();
            var statusDone = status.Where(x => x.Name == StatusConstant.Done).Select(x => x.Id).FirstOrDefault();

            if (export.StatusId == statusPending)
            {
                export.StatusId = statusProcessing;
            }
            else
            {
                export.StatusId = statusDone;
            }

            _repoWrapper.Export.Update(export);
            await _repoWrapper.SaveAsync();
            await _cacheService.RemoveCacheTreeAsync("export");

            return response;
        }

        //public async Task<ExportObjectResponse> CreateFromTicket(string adminId, string forUserId, TicketInfoEntity dto)
        //{
        //    ResultResponse<Export> response = new();

        //    var exportDetails = new List<ExportDetailEntity>();

        //    foreach (var detail in dto.Details!)
        //        exportDetails.Add(new()
        //        {
        //            ItemId = detail.ItemId,
        //            Quantity = detail.Quantity,
        //            Note = detail.Note,
        //        });

        //    ExportEntity export = new()
        //    {
        //        Description = dto.Description,
        //        Status = ExportStatus.Pending,
        //        ForId = forUserId,

        //        CreatedDate = DateTime.UtcNow,
        //        CreatedById = adminId,
        //        UpdatedDate = DateTime.UtcNow,
        //        UpdatedById = adminId,

        //        Details = exportDetails,
        //    };

        //    await _export.AddAsync(export);
        //    await _unitOfWork.SaveAsync();

        //    response.Data = _mapper.Map<Export>(export);
        //    response.StatusCode = ResponseCode.Success;
        //    response.Message = new("Export", "Export created!");

        //    return response;
        //}

        #endregion
    }
}
