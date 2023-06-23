using AutoMapper;
using Inventory.Core.Common;
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

        public async Task<ResultResponse<ExportWithDetailDTO>> CancelExport(int id)
        {
            ResultResponse<ExportWithDetailDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var export = await _export.GetById(id);

            if (export == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Export", "Export not found!"));
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

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Export", "Export canceled!"));
            }
            return response;
        }

        public async Task<ResultResponse<ExportWithDetailDTO>> CreateExport(string token, ExportCreateDTO dto)
        {
            ResultResponse<ExportWithDetailDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            var exportDetails = new List<ExportDetail>();

            foreach (var detail in dto.Details!)
            {
                var item = await _item.GetById(detail.ItemId);

                if (item == null || item.InStock < detail.Quantity)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;

                    if (item == null)
                    {
                        response.Messages.Add(new ResponseMessage("Item", $"Item #{detail.ItemId} not exists!"));
                    }
                    else
                    {
                        response.Messages.Add(new ResponseMessage("Item", $"Out of stock!"));
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

            response.Data = _mapper.Map<ExportWithDetailDTO>(export);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Messages.Add(new ResponseMessage("Export", "Export created!"));

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ExportWithDetailDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<ExportWithDetailDTO>> response = new()
            {  Messages = new List<ResponseMessage>() };

            var exports = await _export.GetAllAsync();

            if (exports.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ExportWithDetailDTO>>(exports);
            }
            else
            {
                response.Status =ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Export", "There is no record"));
            }
            
            return response;
        }

        public async Task<ResultResponse<ExportWithDetailDTO>> GetById(int id)
        {
            ResultResponse<ExportWithDetailDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var export = await _export.GetById(id);

            if(export == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Export", "Export not found!"));
            }
            else
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<ExportWithDetailDTO>(export);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ExportWithDetailDTO>>> GetExportByItemId(Guid id)
        {
            ResultResponse<IEnumerable<ExportWithDetailDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var item = await _item.GetById(id);

            if (item == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", $"Item #{id} not exists!"));
            }
            else
            {
                var exports = await _export.ExportByItem(item);

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ExportWithDetailDTO>>(exports);
            }

            return response;
        }
    }
}
