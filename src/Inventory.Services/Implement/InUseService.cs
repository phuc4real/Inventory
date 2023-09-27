using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository;
using Inventory.Repository.Model;
using Inventory.Service;
using Inventory.Service.Common;
using Microsoft.AspNetCore.Identity;

namespace Inventory.Service.Implement
{
    public class InUseService : IInUseService
    {
        private readonly IMapper _mapper;
        private readonly IExportEntryRepository _exportDetail;
        private readonly UserManager<AppUserEntity> _userManager;
        private readonly ITokenService _tokenService;

        public InUseService(
            IMapper mapper,
            ITokenService tokenService,
            IExportEntryRepository exportDetail,
            UserManager<AppUserEntity> userManager
            )
        {
            _mapper = mapper;
            _userManager = userManager;
            _exportDetail = exportDetail;
            _tokenService = tokenService;
        }

        public async Task<PaginationResponse<InUse>> GetPagination(string token, PaginationRequest request)
        {
            PaginationResponse<InUse> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var userId = _tokenService.GetuserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            ListResponse<ExportDetailEntity>? result;

            if (userRoles.Contains(InventoryRoles.Admin))
            {
                result = await _exportDetail.GetPagination(request);
            }
            else if (userRoles.Contains(InventoryRoles.TeamLeader))
            {
                result = await _exportDetail.GetPagination(request, user!.TeamId!.Value);
            }
            else
            {
                result = await _exportDetail.GetPagination(request, userId);
            }

            if (result.Data!.Any())
            {
                response.TotalRecords = result.TotalRecords;
                response.TotalPages = result.TotalPages;
                response.StatusCode = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<InUse>>(result.Data);
            }
            else
            {
                response.StatusCode = ResponseCode.NoContent;
            }

            return response;
        }
    }
}
