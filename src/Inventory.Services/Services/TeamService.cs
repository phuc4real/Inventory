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

namespace Inventory.Services.Services
{
    public class TeamService : ITeamServices
    {
        private readonly ITeamRepository _team;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;

        public TeamService(
            ITeamRepository team,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            UserManager<AppUser> userManager)
        {
            _team = team;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<ResultResponse<TeamDTO>> AddMember(string token, Guid teamId, string memberId)
        {
            ResultResponse<TeamDTO> response = new ();

            var userIdFromToken = _tokenService.GetUserId(token);
            var newMember = await _userManager.FindByIdAsync(memberId);

            var team = await _team.GetById(teamId);

            if (team == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Team", "Team not exists!");
            }
            else
            {
                if (userIdFromToken != team.LeaderId)
                {
                    response.Status = ResponseCode.Forbidden;
                    response.Message = new("Team", $"You are not leader of Team {team.Name}!");
                }
                else
                {
                    if (team.Members != null)
                    {
                        team.Members!.Add(newMember!);
                    }
                    else
                    {
                        team.Members = new List<AppUser>
                        {
                            newMember!
                        };
                    }

                    _team.Update(team);
                    await _unitOfWork.SaveAsync();
                    response.Status = ResponseCode.Success;
                    response.Message = new("Team", "Add new member to team successfully!");
                }

            }

            return response;
        }

        public async Task<ResultResponse<TeamDTO>> CreateTeam(string token, TeamEditDTO dto)
        {
            ResultResponse<TeamDTO> response = new()
            ;

            var userId = _tokenService.GetUserId(token);

            Team team = _mapper.Map<Team>(dto);
            team.LeaderId = userId;

            await _team.AddAsync(team);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<TeamDTO>(team);
            response.Status = ResponseCode.Success;
            response.Message = new("Team", "Team created!");
            return response;
        }

        public async Task<ResultResponse<TeamDTO>> DeleteTeam(string token, Guid id)
        {
            ResultResponse<TeamDTO> response = new()
            ;
            
            var userId = _tokenService.GetUserId(token);

            var team = await _team.GetById(id);

            if (team == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Team", "Team not exists!");
            }
            else
            {
                if(userId != team.LeaderId)
                {
                    response.Status = ResponseCode.Forbidden;
                    response.Message = new("Team", $"You are not leader of Team {team.Name}!");
                }
                else
                {
                    _team.Remove(team);
                    await _unitOfWork.SaveAsync();

                    response.Status = ResponseCode.Success;
                    response.Message = new("Team", "Team deleted!");
                }
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TeamDTO>>> GetList()
        {
            ResultResponse<IEnumerable<TeamDTO>> response = new();

            var teams = await _team.GetList();

            if (teams.Any())
            {
                response.Data = _mapper.Map<IEnumerable<TeamDTO>>(teams);
                response.Status = ResponseCode.Success;
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                //response.Message = new("Team", "There is no record");
            }

            return response;
        }

        public async Task<ResultResponse<TeamWithMembersDTO>> GetById(Guid id)
        {
            ResultResponse<TeamWithMembersDTO> response = new() 
            ;

            var team = await _team.GetById(id);

            if (team == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Team", "Team not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<TeamWithMembersDTO>(team);
            }

            return response;
        }

        public async Task<ResultResponse<TeamDTO>> UpdateTeam(string token, Guid id, TeamEditDTO dto)
        {
            ResultResponse<TeamDTO> response = new() 
            ;

            var userId = _tokenService.GetUserId(token);

            var team = await _team.GetById(id);

            if (team == null)
            {
                response.Status =ResponseCode.NotFound;
                response.Message = new("Team", "Team not found!");
            }
            else
            {
                if (userId != team.LeaderId)
                {
                    response.Status = ResponseCode.Forbidden;
                    response.Message = new("Team", $"You are not leader of Team {team.Name}!");
                }
                else
                {
                    team.Name = dto.Name;
                    team.LeaderId = dto.LeaderId;

                    _team.Update(team);
                    await _unitOfWork.SaveAsync();

                    response.Status = ResponseCode.Success;
                    response.Message = new("Team", "Team updated!");
                }
            }

            return response;
        }

        public async Task<PaginationResponse<TeamDTO>> GetPagination(PaginationRequest request)
        {
            PaginationResponse<TeamDTO> response = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            var lists = await _team.GetPagination(request);

            if (lists.Data!.Any())
            {
                response.TotalRecords = lists.TotalRecords;
                response.TotalPages = lists.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<TeamDTO>>(lists.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                //response.Message = new("Team", "No record!");
            }

            return response;
        }
    }
}
