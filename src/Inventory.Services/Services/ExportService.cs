using AutoMapper;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;

namespace Inventory.Services.Services
{
    public class ExportService : IExportService
    {
        private readonly IExportRepository _export;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IItemService _itemService;
        private readonly ITokenService _tokenService;

        public ExportService(
            IExportRepository export,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IItemService itemService,
            ITokenService tokenService)
        {
            _export = export;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _itemService = itemService;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<Export>> CreateFromTicket(string adminId, string forUserId, TicketInfoEntity dto)
        {
            ResultResponse<Export> response = new();

            var exportDetails = new List<ExportDetailEntity>();

            foreach (var detail in dto.Details!)
                exportDetails.Add(new()
                {
                    ItemId = detail.ItemId,
                    Quantity = detail.Quantity,
                    Note = detail.Note,
                });

            ExportEntity export = new()
            {
                Description = dto.Description,
                Status = ExportStatus.Pending,
                ForId = forUserId,

                CreatedDate = DateTime.UtcNow,
                CreatedById = adminId,
                UpdatedDate = DateTime.UtcNow,
                UpdatedById = adminId,

                Details = exportDetails,
            };

            await _export.AddAsync(export);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<Export>(export);
            response.Status = ResponseCode.Success;
            response.Message = new("Export", "Export created!");

            return response;
        }

        public async Task<ResultResponse<Export>> GetById(int id)
        {
            ResultResponse<Export> response = new();

            var export = await _export.GetById(id);

            if (export == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Export", "Export not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<Export>(export);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<ResponseMessage>>> GetCountByMonth()
        {
            ResultResponse<IEnumerable<ResponseMessage>> response = new();

            var result = await _export.GetCount();

            response.Status = ResponseCode.Success;
            response.Data = result;

            return response;
        }

        public async Task<ResultResponse<IEnumerable<Export>>> GetList()
        {
            ResultResponse<IEnumerable<Export>> response = new();

            var exports = await _export.GetList();

            if (exports.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Export>>(exports);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }

        public async Task<PaginationResponse<Export>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<Export> response = new()
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
                response.Data = _mapper.Map<IEnumerable<Export>>(lists.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }

        public async Task<ResultResponse> UpdateStatus(string token, int id)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var export = await _export.GetById(id);

            if (export == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Export", "Export not found");
            }
            else
                switch (export.Status)
                {
                    case ExportStatus.Pending:
                        var res = await _itemService.Export(
                            export.Details!
                                .ToList());

                        if (res.Status != ResponseCode.Success)
                            return res;

                        export.Status = ExportStatus.Processing;
                        export.UpdatedDate = DateTime.UtcNow;
                        export.UpdatedById = userId;

                        _export.Update(export);
                        await _unitOfWork.SaveAsync();

                        //send mail to user
                        break;

                    case ExportStatus.Processing:
                        export.Status = ExportStatus.Processing;
                        export.UpdatedDate = DateTime.UtcNow;
                        export.UpdatedById = userId;

                        _export.Update(export);
                        await _unitOfWork.SaveAsync();

                        //send mail to user
                        break;
                    case ExportStatus.Done:
                        response.Message = new("Export", "Export already done!");
                        response.Status = ResponseCode.BadRequest;
                        break;
                }

            return response;
        }
    }
}
