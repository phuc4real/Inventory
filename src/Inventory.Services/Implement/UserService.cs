using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Constants;
using Inventory.Core.Enums;
using Inventory.Core.Extensions;
using Inventory.Model.Entity;
using Inventory.Service.DTO.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<UserObjectResponse> GetByUserNameAsync(string userName)
        {
            UserObjectResponse response = new();

            var user = await _userManager.FindByNameAsync(userName);

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

            var user = await _userManager.FindByNameAsync(request.GetUserContext());

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

        public async Task<UserPermission> CheckRoleOfUser(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var roles = await _userManager.GetRolesAsync(user);

            var result = new UserPermission
            {
                IsSuperAdmin = roles.Contains(InventoryRoles.SuperAdmin),
                IsAdmin = roles.Contains(InventoryRoles.Admin),
            };

            return result;
        }

        public async Task<Operation> GetOperationAsync(BaseRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.GetUserContext());
            var roles = await _userManager.GetRolesAsync(user);
            var operation = GetOperationOfUser(roles);

            return operation;
        }

        #endregion

        #region Private

        private Operation GetOperationOfUser(IList<string> roles)
        {
            var isSuperAdmin = roles.Contains(InventoryRoles.SuperAdmin);
            var isAdmin = roles.Contains(InventoryRoles.Admin);
            var isAdminOrSuperAdmin = isAdmin || isSuperAdmin;

            var operation = new Operation()
            {
                Item = new() { CanView = true, CanEdit = isAdminOrSuperAdmin, },
                Dashboard = new() { CanView = isAdminOrSuperAdmin },
                Category = new() { CanView = true, CanEdit = isAdminOrSuperAdmin, },
                Order = new() { CanView = isAdminOrSuperAdmin, CanEdit = isAdminOrSuperAdmin, CanApproval = isSuperAdmin },
                Export = new() { CanView = isAdminOrSuperAdmin, CanEdit = isAdminOrSuperAdmin, },
                Ticket = new() { CanView = true, CanEdit = true, CanChangeStatus = isAdminOrSuperAdmin, CanApproval = isSuperAdmin },
                ItemHolder = new() { CanView = true, CanEdit = isAdminOrSuperAdmin },
            };


            return operation;
        }

        #endregion
    }
}
