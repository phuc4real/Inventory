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

        public ExportService(
            IExportRepository export,
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _export = export;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultResponse<IEnumerable<ExportDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<ExportDTO>> response = new()
            {  Messages = new List<ResponseMessage>() };

            var exports = await _export.GetAllAsync();

            if (exports.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ExportDTO>>(exports);
            }
            else
            {
                response.Status =ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Export", "There is no export record"));
            }
            
            return response;
        }
    }
}
