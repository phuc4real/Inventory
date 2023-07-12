using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;

namespace Inventory.Services.Services
{
    public class UsingItemService : IUsingItemService
    {
        private readonly IExportDetailRepository _exportDetail;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;

        public UsingItemService(
            IExportDetailRepository exportDetail,
            IMapper mapper,
            UserManager<AppUser> userManager,
            ITokenService tokenService)
        {
            _exportDetail = exportDetail;
            _mapper = mapper;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetList(string token)
        {
            ResultResponse<IEnumerable<UsingItemDTO>> response = new()
            ;

            var userId = _tokenService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            IEnumerable<ExportDetail>? result;

            if (userRoles.Contains(InventoryRoles.IM))
            {
                result = await _exportDetail.GetList();
            }
            else if (userRoles.Contains(InventoryRoles.PM))
            {
                result = await _exportDetail.GetList(user!.TeamId!.Value);
            }
            else
            {
                result = await _exportDetail.GetList(userId);
            }

            if (result.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<UsingItemDTO>>(result);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("UsingItem", "There is no record!");
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetMyUsingItem(string token)
        {
            ResultResponse<IEnumerable<UsingItemDTO>> response = new()
            ;

            var userId = _tokenService.GetUserId(token);

            var result = await _exportDetail.GetList(userId);

            if (result.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<UsingItemDTO>>(result);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                response.Message = new("UsingItem", "There is no record!");
            }

            return response;
        }

        public async Task<PaginationResponse<UsingItemDTO>> GetPagination(string token, PaginationRequest request)
        {
            PaginationResponse<UsingItemDTO> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var lists = await _exportDetail.GetPagination(request);

            if (lists.Data!.Any())
            {
                response.TotalRecords = lists.TotalRecords;
                response.TotalPages = lists.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<UsingItemDTO>>(lists.Data);
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
