using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Repository.Repositories;
using Inventory.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class ExportService : IExportService
    {
        private readonly IExportRepository _export;
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public ExportService(
            IExportRepository export,
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService)
        {
            _export = export;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<ExportDTO>> CancelExport(int id)
        {
            ResultResponse<ExportDTO> response = new();

            var export = await _export.GetById(id);

            if (export == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Export", "Export not found!");
            }
            else
            {
                foreach(var detail in export.Details!)
                {
                    var item =await _item.GetById(detail.ItemId);
                    item.Used -= detail.Quantity;
                    item.InStock += detail.Quantity;

                    _item.Update(item);
                }

                export.IsCancel = true;
                _export.Update(export);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new ("Export", "Export canceled!");
            }
            return response;
        }

        public async Task<ResultResponse<ExportDTO>> CreateExport(string token, ExportCreateDTO dto)
        {
            ResultResponse<ExportDTO> response = new();

            var userId = _tokenService.GetUserId(token);

            var exportDetails = new List<ExportDetail>();

            foreach (var detail in dto.Details!)
            {
                var item = await _item.GetById(detail.ItemId);

                if (item == null || item.InStock < detail.Quantity)
                {
                    if (item == null)
                    {
                        response.Status = ResponseCode.NotFound;
                        response.Message = new ("Item", $"Item #{detail.ItemId} not exists!");
                    }
                    else
                    {
                        response.Status = ResponseCode.UnprocessableContent;
                        response.Message = new ("Item", $"Out of stock!");
                    }

                    return response;
                }

                item.Used += detail.Quantity;
                item.InStock -= detail.Quantity;
                _item.Update(item);

                exportDetails.Add(_mapper.Map<ExportDetail>(detail));
            }

            Export export = new()
            {
                Description = dto.Description,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId,
                Details = exportDetails,
                IsCancel = false
            };

            await _export.AddAsync(export);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<ExportDTO>(export);
            response.Status = ResponseCode.Success;
            response.Message = new ("Export", "Export created!");

            return response;
        }

        public async Task<ResultResponse<ExportDTO>> GetById(int id)
        {
            ResultResponse<ExportDTO> response = new();

            var export = await _export.GetById(id);

            if(export == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Export", "Export not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<ExportDTO>(export);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ExportDTO>>> GetList()
        {
            ResultResponse<IEnumerable<ExportDTO>> response = new();

            var exports = await _export.GetList();

            if (exports.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<ExportDTO>>(exports);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("Export", "There is no record");
            }

            return response;
        }

        public async Task<PaginationResponse<ExportDTO>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<ExportDTO> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var lists = await _export.GetPagination(request);

            if (lists.Data!.Any())
            {
                response.TotalRecords = lists.TotalRecords;
                response.TotalPages = lists.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<ExportDTO>>(lists.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("Export", "No record!");
            }

            return response;
        }
    }
}
