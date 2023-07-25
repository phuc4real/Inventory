using AutoMapper;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Services.IServices;

namespace Inventory.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _user;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public UserService(
            IUserRepository user,
            IMapper mapper,
            ITokenService tokenService)
        {
            _user = user;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<AppUserDetail>> GetById(string id)
        {
            ResultResponse<AppUserDetail> response = new();

            var user = await _user.GetById(id);

            if (user == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("User", "User not exist");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<AppUserDetail>(user);
                response.Data.Permission = await _user.CheckRole(id); ;
            }

            return response;
        }

        public async Task<ResultResponse<AppUserDetail>> GetByToken(string token)
        {
            ResultResponse<AppUserDetail> response = new();

            var userId = _tokenService.GetuserId(token);
            var user = await _user.GetById(userId);

            if (user == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("User", "User not exist");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<AppUserDetail>(user);
                response.Data.Permission = await _user.CheckRole(userId); ;
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<AppUser>>> GetList()
        {
            ResultResponse<IEnumerable<AppUser>> response = new();

            var list = await _user.GetList();

            if (list.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<AppUser>>(list);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }
    }
}
