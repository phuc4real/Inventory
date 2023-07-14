using AutoMapper;
using Inventory.Core.Enums;
using Inventory.Core.Request;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _user;
        private readonly IMapper _mapper;

        public UserService(IUserRepository user, IMapper mapper)
        {
            _user = user;
            _mapper = mapper;
        }
        public async Task<ResultResponse<AppUserWithTeamDTO>> GetById(string id)
        {
            ResultResponse<AppUserWithTeamDTO> response = new();

            var user = await _user.GetById(id);

            if (user == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("User", "User not exist");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<AppUserWithTeamDTO>(user);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<AppUserDTO>>> GetList()
        {
            ResultResponse<IEnumerable<AppUserDTO>> response = new();

            var list = await _user.GetList();

            if (list.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<AppUserDTO>>(list);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }

        public async Task<PaginationResponse<AppUserDTO>> GetPagination(PaginationRequest request)
        {

            PaginationResponse<AppUserDTO> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var list = await _user.GetPagination(request);

            if (list.Data!.Any())
            {
                response.TotalRecords = list.TotalRecords;
                response.TotalPages = list.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<AppUserDTO>>(list.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }
    }
}
