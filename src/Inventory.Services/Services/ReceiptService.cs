using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receipt;
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public ReceiptService(
            IReceiptRepository receipt,
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService)
        {
            _receipt = receipt;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }
        public async Task<ResultResponse<ReceiptDTO>> CreateReceipt(string token, ReceiptCreateDTO dto)
        {
            ResultResponse<ReceiptDTO> response = new()
                { Messages = new List<ResponseMessage>() };

            var userid = _tokenService.GetUserId(token);
            List<ReceiptDetail> details = new();

            foreach (var detail in dto.Details!)
            {
                var itemExists = await _item.AnyAsync(x => x.Id == detail.ItemId);

                if (!itemExists)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Item", $"Item #{detail.ItemId} not exists!"));

                    return response;
                }

                details.Add(_mapper.Map<ReceiptDetail>(detail));
            }

            Receipt receipt = new()
            {
                ItemCount = dto.ItemCount,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userid,
                Details = details
            };
            
            await _receipt.AddAsync(receipt);
            await _unitOfWork.SaveAsync();

            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Messages.Add(new ResponseMessage("Receipt", "Receipt Created!"));
            response.Data = _mapper.Map<ReceiptDTO>(receipt);

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ReceiptDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<ReceiptDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var receipts = await _receipt.GetAllAsync();

            if(receipts.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Receipt", "There is no record!"));
            }

            return response;
        }

        public async Task<ResultResponse<ReceiptDTO>> ReceiptById(int id)
        {
            ResultResponse<ReceiptDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var receipt = await _receipt.GetById(id);

            if(receipt == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Receipt", "Receip not found!"));
            }
            else
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<ReceiptDTO>(receipt);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ReceiptDTO>>> ReceiptByItemId(Guid itemId)
        {
            ResultResponse<IEnumerable<ReceiptDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var item = await _item.GetById(itemId);

            if (item == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", $"Item #{itemId} not found!"));
            }
            else
            {
                var receipts = await _receipt.ReceiptByItem(item!);

                if (receipts.Any())
                {
                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    response.Data = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);
                }
                else
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Receipt", "There is no record!"));
                }
            }

            return response;
        }
    }
}
