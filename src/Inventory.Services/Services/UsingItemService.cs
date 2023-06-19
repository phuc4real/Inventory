using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class UsingItemService : IUsingItemService
    {
        private readonly IExportDetailRepository _exportDetail;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsingItemService(IExportDetailRepository exportDetail, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _exportDetail = exportDetail;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetAllUsingItemAsync()
        {
            ResultResponse<IEnumerable<UsingItemDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var result = await _exportDetail.GetAllAsync();

            if (result.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<UsingItemDTO>>(result);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("UsingItem", "There is no record!"));
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<UsingItemDTO>>> SearchForUsingItemAsync(string filter)
        {
            ResultResponse<IEnumerable<UsingItemDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var result = await _exportDetail.SearchAsync(filter);

            if (result.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<UsingItemDTO>>(result);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("UsingItem", "There is no record!"));
            }

            return response;
        }
    }
}
