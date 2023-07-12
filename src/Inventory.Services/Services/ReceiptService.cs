using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;

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
            ResultResponse<ReceiptDTO> response = new();

            var userId = _tokenService.GetUserId(token);
            List<ReceiptDetail> details = new();

            foreach (var detail in dto.Details!)
            {
                var item = await _item.GetById(detail.ItemId);

                if (item == null)
                {
                    response.Status = ResponseCode.NotFound;
                    response.Message = new("Item", $"Item #{detail.ItemId} not exists!");

                    return response;
                }

                item.InStock += detail.Quantity;
                _item.Update(item);
                details.Add(_mapper.Map<ReceiptDetail>(detail));
            }

            Receipt receipt = new()
            {
                ItemCount = dto.ItemCount,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId,
                Details = details
            };
            
            await _receipt.AddAsync(receipt);
            await _unitOfWork.SaveAsync();

            response.Status = ResponseCode.Success;
            response.Message = new("Receipt", "Receipt Created!");
            response.Data = _mapper.Map<ReceiptDTO>(receipt);

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ReceiptDTO>>> GetList()
        {
            ResultResponse<IEnumerable<ReceiptDTO>> response = new();

            var receipts = await _receipt.GetList();

            if(receipts.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<ReceiptDTO>>(receipts);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("Receipt", "There is no record!");
            }

            return response;
        }

        public async Task<PaginationResponse<ReceiptDTO>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<ReceiptDTO> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var lists = await _receipt.GetPagination(request);

            if (lists.Data!.Any())
            {
                response.TotalRecords = lists.TotalRecords;
                response.TotalPages = lists.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<ReceiptDTO>>(lists.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("Receipt", "No record!");
            }

            return response;
        }

        public async Task<ResultResponse<ReceiptDTO>> ReceiptById(int id)
        {
            ResultResponse<ReceiptDTO> response = new();

            var receipt = await _receipt.GetById(id);

            if(receipt == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Receipt", "Receip not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<ReceiptDTO>(receipt);
            }

            return response;
        }
    }
}
