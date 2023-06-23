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
            ResultResponse<TeamDTO> response = new ()
            { Messages = new List<ResponseMessage>() };

            var userIdFromToken = _tokenService.GetUserId(token);
            var newMember = await _userManager.FindByIdAsync(memberId);

            var team = await _team.GetTeamById(teamId);

            if (team == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not exists!"));
            }
            else
            {
                if (userIdFromToken != team.LeaderId)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Team", $"You are not leader of Team {team.Name}!"));
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
                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    response.Messages.Add(new ResponseMessage("Team", "Add new member to team successfully!"));
                }

            }

            return response;
        }

        public async Task<ResultResponse<TeamDTO>> CreateTeam(string token, TeamEditDTO dto)
        {
            ResultResponse<TeamDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            Team team = _mapper.Map<Team>(dto);
            team.LeaderId = userId;

            await _team.AddAsync(team);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<TeamDTO>(team);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Messages.Add(new ResponseMessage("Team", "Team created!"));
            return response;
        }

        public async Task<ResultResponse<TeamDTO>> DeleteTeam(string token, Guid id)
        {
            ResultResponse<TeamDTO> response = new()
            { Messages = new List<ResponseMessage>() };
            
            var userId = _tokenService.GetUserId(token);

            var team = await _team.GetTeamById(id);

            if (team == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not exists!"));
            }
            else
            {
                if(userId != team.LeaderId)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Team", $"You are not leader of Team {team.Name}!"));
                }
                else
                {
                    _team.Remove(team);
                    await _unitOfWork.SaveAsync();

                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    response.Messages.Add(new ResponseMessage("Team", "Team deleted!"));
                }
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TeamDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<TeamDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var teams = await _team.GetAllWithPropertyAsync();

            if (teams.Any())
            {
                response.Data = _mapper.Map<IEnumerable<TeamDTO>>(teams);
                response.Status = ResponseStatus.STATUS_SUCCESS;
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "There is no record"));
            }

            return response;
        }

        public async Task<ResultResponse<TeamWithMembersDTO>> GetById(Guid id)
        {
            ResultResponse<TeamWithMembersDTO> response = new() 
            { Messages = new List<ResponseMessage>() };

            var team = await _team.GetTeamById(id);

            if (team == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not found!"));
            }
            else
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<TeamWithMembersDTO>(team);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TeamDTO>>> SearchTeamByName(string name)
        {
            ResultResponse<IEnumerable<TeamDTO>> response = new() 
            { Messages = new List<ResponseMessage>() };

            var teams = await _team.GetAsync(x=> x.Name!.Contains(name));

            if (teams.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<TeamDTO>>(teams);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not found!"));
            }

            return response;
        }

        public async Task<ResultResponse<TeamDTO>> UpdateTeam(string token, Guid id, TeamEditDTO dto)
        {
            ResultResponse<TeamDTO> response = new() 
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            var team = await _team.GetTeamById(id);

            if (team == null)
            {
                response.Status =ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not found!"));
            }
            else
            {
                if (userId != team.LeaderId)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Team", $"You are not leader of Team {team.Name}!"));
                }
                else
                {
                    team.Name = dto.Name;
                    team.LeaderId = dto.LeaderId;

                    _team.Update(team);
                    await _unitOfWork.SaveAsync();

                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    response.Messages.Add(new ResponseMessage("Team", "Team updated!"));
                }
            }

            return response;
        }
    }
}
