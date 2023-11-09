using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Repository;
using Inventory.Service.DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace Inventory.Service.Implement
{
    public class UserService : IUserService
    {

        #region Ctor & Field

        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(
            UserManager<AppUser> userManager,
            IMapper mapper,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        #endregion

        #region Method

        public async Task<UserObjectResponse> GetByIdAsync(string id)
        {
            UserObjectResponse response = new();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("User", "User not exist");
            }
            else
            {
                response.Data = _mapper.Map<UserResponse>(user);
            }

            return response;
        }

        public async Task<UserObjectResponse> GetAsync(BaseRequest request)
        {
            UserObjectResponse response = new();

            var userName = _tokenService.GetUserNameFromToken(request.GetToken());
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                response.StatusCode = ResponseCode.BadRequest;
                response.Message = new("User", "User not exist");
            }
            else
            {
                response.Data = _mapper.Map<UserResponse>(user);

                var roles = await _userManager.GetRolesAsync(user);
                response.Data.Permission = new UserPermission
                {
                    IsSuperAdmin = roles.Contains(InventoryRoles.SuperAdmin),
                    IsAdmin = roles.Contains(InventoryRoles.Admin),
                };
            }

            return response;
        }

        public async Task<UserPaginationResponse> GetListAsync(PaginationRequest request)
        {
            UserPaginationResponse response = new();

            var users = await _userManager.Users.ToListAsync();

            if (request.SearchKeyword != null)
            {
                var searchToLower = request.SearchKeyword.ToLower();
                users = users.Where(x => x.UserName!.Contains(searchToLower) || x.Email!.Contains(searchToLower)).ToList();
            }

            response.Count = users.Count;

            var result = await users.AsQueryable().Pagination(request).ToListAsync();

            response.Data = _mapper.Map<List<UserResponse>>(result);

            return response;
        }

        public async Task<UserPermission> CheckRoleOfUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            var result = new UserPermission
            {
                IsSuperAdmin = roles.Contains(InventoryRoles.SuperAdmin),
                IsAdmin = roles.Contains(InventoryRoles.Admin),
            };

            return result;
        }

        #endregion
    }
}
