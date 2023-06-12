using AutoMapper;
using Inventory.Core.Common;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;

namespace Inventory.Services.Services
{
    public class TeamService : ITeamServices
    {
        private readonly ITeamRepository _team;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamService(ITeamRepository team, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _team = team;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultResponse<TeamDTO>> CreateTeam(TeamEditDTO dto)
        {
            ResultResponse<TeamDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            Team team = _mapper.Map<Team>(dto);

            await _team.AddAsync(team);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<TeamDTO>(team);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Messages.Add(new ResponseMessage("Team", "Team created!"));
            return response;

        }

        public async Task<ResultResponse<TeamDTO>> DeleteTeam(Guid id)
        {
            ResultResponse<TeamDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var team = await _team.GetTeamById(id);

            if (team == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not exists!"));
            }
            else
            {
                _team.Remove(team);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Team", "Team deleted!"));
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TeamDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<TeamDTO>> response = new();

            var teams = await _team.GetAllWithPropertyAsync();

            response.Data = _mapper.Map<IEnumerable<TeamDTO>>(teams);
            response.Status = ResponseStatus.STATUS_SUCCESS;

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

        public async Task<ResultResponse<TeamDTO>> UpdateTeam(Guid id, TeamEditDTO dto)
        {
            ResultResponse<TeamDTO> response = new() 
            { Messages = new List<ResponseMessage>() };

            var team = await _team.GetTeamById(id);

            if (team == null)
            {
                response.Status =ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Team", "Team not found!"));
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

            return response;
        }
    }
}
