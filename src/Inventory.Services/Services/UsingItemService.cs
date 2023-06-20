using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Identity;

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

        public async Task<ResultResponse<IEnumerable<UsingItemDTO>>> GetUsingItemByRole(string token)
        {
            ResultResponse<IEnumerable<UsingItemDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            IEnumerable<ExportDetail>? result;

            if (userRoles.Contains(InventoryRoles.IM))
            {
                result = await _exportDetail.GetUsingItem();
            }
            else if (userRoles.Contains(InventoryRoles.PM))
            {
                result = await _exportDetail.GetUsingItemByTeam(user!.TeamId!.Value);
            }
            else
            {
                result = await _exportDetail.GetUsingItemByUser(userId);
            }

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

        public async Task<ResultResponse<IEnumerable<UsingItemDTO>>> MyUsingItem(string token)
        {
            ResultResponse<IEnumerable<UsingItemDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            var result = await _exportDetail.GetUsingItemByUser(userId);

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
