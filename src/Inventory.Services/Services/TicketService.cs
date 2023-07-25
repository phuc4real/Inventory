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
using StackExchange.Redis;

namespace Inventory.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketRepository _ticket;
        private readonly ITicketInfoRepository _ticketInfo;
        private readonly IItemService _itemService;
        private readonly ITokenService _tokenService;
        private readonly IExportService _exportService;
        private readonly UserManager<AppUserEntity> _userManager;

        public TicketService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            ITicketRepository ticket,
            ITicketInfoRepository ticketInfo,
            IItemService itemService,
            ITokenService tokenService,
            IExportService exportService,
            UserManager<AppUserEntity> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _ticket = ticket;
            _ticketInfo = ticketInfo;
            _itemService = itemService;
            _tokenService = tokenService;
            _exportService = exportService;
            _userManager = userManager;
        }

        public async Task<PaginationResponse<Ticket>> GetPagination(string token, PaginationRequest request)
        {
            PaginationResponse<Ticket> response = new();

            var userId = _tokenService.GetuserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            PaginationList<TicketEntity>? tickets;

            if (userRoles.Contains(InventoryRoles.Admin))
            {
                tickets = await _ticket.GetPagination(request);
            }
            else if (userRoles.Contains(InventoryRoles.TeamLeader))
            {
                tickets = await _ticket.GetPagination(request, user!.TeamId!.Value);
            }
            else
            {
                tickets = await _ticket.GetPagination(request, userId);
            }

            if (tickets.Data!.Any())
            {
                response.TotalRecords = tickets.TotalRecords;
                response.TotalPages = tickets.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Ticket>>(tickets.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<Ticket>>> GetList()
        {
            ResultResponse<IEnumerable<Ticket>> response = new();

            var list = await _ticket.GetList();

            if (list.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<Ticket>>(list);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }

        public async Task<ResultResponse<Ticket>> GetById(string token, int id)
        {
            ResultResponse<Ticket> response = new();

            var userId = _tokenService.GetuserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            TicketEntity? ticket = await _ticket.GetById(id);

            if (!userRoles.Contains(InventoryRoles.Admin))
            {
                if (userRoles.Contains(InventoryRoles.TeamLeader))
                {
                    if (ticket.CreatedByUser!.TeamId != user!.TeamId)
                    {
                        ticket = null;
                    }
                }
                else
                {
                    if (ticket.CreatedById != userId)
                    {
                        ticket = null;
                    }
                }
            }

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<Ticket>(ticket);
            }

            return response;
        }

        public async Task<ResultResponse<Ticket>> Create(string token, UpdateTicketInfo dto)
        {
            ResultResponse<Ticket> response = new();

            var userId = _tokenService.GetuserId(token);
            var res = await _itemService.Exists(dto.Details!.Select(x => x.ItemId).ToList());

            if (res.Status != ResponseCode.Success)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = res.Message;
                return response;
            }

            TicketInfoEntity ticketInfo = new()
            {
                Purpose = dto.Purpose,
                Title = dto.Title,
                Description = dto.Description,
                LeaderDecision = new() { Status = DecisionStatus.Pending },
                Decision = new() { Status = DecisionStatus.Pending },
                Details = _mapper.Map<IList<TicketDetailEntity>>(dto.Details)
            };

            TicketEntity ticket = new()
            {
                CreatedDate = DateTime.UtcNow,
                CreatedById = userId,
                UpdatedDate = DateTime.UtcNow,
                UpdatedById = userId,
                History = new List<TicketInfoEntity>()
            };

            ticket.History.Add(ticketInfo);

            await _ticket.AddAsync(ticket);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<Ticket>(ticket);
            response.Status = ResponseCode.Created;
            response.Message = new("Ticket", "Ticket created!");

            return response;
        }

        public async Task<ResultResponse> Cancel(string token, int id)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var ticket = await _ticket.GetById(id);

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                var ticketInfo = ticket.History!.OrderByDescending(x => x.CreatedAt).First();

                if (ticket.CreatedById != userId)
                {
                    response.Status = ResponseCode.Forbidden;
                    response.Message = new("Forbidden", "You don't have access");
                }
                else
                {
                    if (ticket.CloseDate != null)
                    {
                        response.Status = ResponseCode.BadRequest;
                        response.Message = new("Ticket", "Ticket already closed");
                    }
                    else
                    {
                        ticket.CloseDate = DateTime.UtcNow;
                        ticket.UpdatedDate = DateTime.UtcNow;
                        ticket.UpdatedById = userId;

                        ticketInfo.CloseAt = DateTime.UtcNow;
                        ticketInfo.Status = TicketStatus.Close;

                        _ticketInfo.Update(ticketInfo);
                        _ticket.Update(ticket);
                        await _unitOfWork.SaveAsync();

                        response.Status = ResponseCode.Success;
                        response.Message = new("Ticket", "Cancel ticket success!");
                    }
                }
            }
            return response;
        }

        public async Task<ResultResponse> LeaderDecide(string token, int id, UpdateDecision decision)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var ticket = await _ticket.GetById(id);

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                var ticketInfo = ticket.History!.OrderByDescending(x => x.CreatedAt).First();

                ticketInfo.LeaderDecision = new()
                {
                    Status = decision.Status,
                    ById = userId,
                    Date = DateTime.UtcNow,
                    Message = decision.Message
                };

                ticket.UpdatedDate = DateTime.UtcNow;
                ticket.UpdatedById = userId;

                _ticketInfo.Update(ticketInfo);
                _ticket.Update(ticket);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Decision", "Success");
            }
            return response;
        }

        public async Task<ResultResponse> Decide(string token, int id, UpdateDecision decision)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var ticket = await _ticket.GetById(id);

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                var ticketInfo = ticket.History!.OrderByDescending(x => x.CreatedAt).First();

                ticketInfo.Decision = new()
                {
                    Status = decision.Status,
                    ById = userId,
                    Date = DateTime.UtcNow,
                    Message = decision.Message
                };

                ticket.UpdatedDate = DateTime.UtcNow;
                ticket.UpdatedById = userId;

                _ticketInfo.Update(ticketInfo);
                _ticket.Update(ticket);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Message = new("Decision", "Success");
            }
            return response;
        }

        public async Task<ResultResponse> UpdateStatus(string token, int id)
        {
            ResultResponse response = new();

            var userId = _tokenService.GetuserId(token);
            var ticket = await _ticket.GetById(id);

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                var ticketInfo = ticket.History!.OrderByDescending(x => x.CreatedAt).First();

                bool isLeaderApprove = ticketInfo.LeaderDecision!.Status == DecisionStatus.Approve;
                bool isAdminApprove = ticketInfo.Decision!.Status == DecisionStatus.Approve;

                if (isLeaderApprove && isAdminApprove)
                {
                    switch (ticketInfo.Status)
                    {
                        case TicketStatus.Pending:
                            ticketInfo.Status++;
                            ticket.UpdatedDate = DateTime.UtcNow;
                            ticket.UpdatedById = userId;

                            _ticketInfo.Update(ticketInfo);
                            _ticket.Update(ticket);
                            await _unitOfWork.SaveAsync();
                            await _exportService.CreateFromTicket(userId, ticket.CreatedById!, ticketInfo);

                            response.Status = ResponseCode.Success;
                            response.Message = new("Ticket", "Change status successfully!");
                            break;

                        case TicketStatus.Processing:
                            ticketInfo.Status++;
                            ticket.UpdatedDate = DateTime.UtcNow;
                            ticket.UpdatedById = userId;

                            _ticketInfo.Update(ticketInfo);
                            _ticket.Update(ticket);
                            await _unitOfWork.SaveAsync();

                            response.Status = ResponseCode.Success;
                            response.Message = new("Ticket", "Change status successfully!");
                            break;

                        case TicketStatus.Reject:
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Ticket", "Ticket is rejected!");
                            break;

                        default:
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Ticket", "Ticket is closed!");
                            break;
                    }
                }
                else
                {
                    response.Status = ResponseCode.BadRequest;
                    response.Message = new("Ticket", "Please, waiting for Team Leader & Admin approve the ticket!!");
                }
            }

            return response;
        }

        public async Task<ResultResponse<TicketCount>> GetTicketCount()
        {
            ResultResponse<TicketCount> response = new()
            {
                Data = await _ticket.GetCount(),
                Status = ResponseCode.Success
            };

            return response;
        }
    }
}
