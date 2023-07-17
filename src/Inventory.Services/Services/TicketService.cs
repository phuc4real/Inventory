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
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticket;
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<AppUser> _userManager;

        public TicketService(
            ITicketRepository ticket,
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService,
            UserManager<AppUser> userManager)
        {
            _ticket = ticket;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<ResultResponse<TicketDTO>> RejectTicket(string token, Guid ticketId, string rejectReason)
        {
            ResultResponse<TicketDTO> response = new();

            var userId = _tokenService.GetUserId(token);

            var ticket = await _ticket.GetById(ticketId);

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                ticket.IsClosed = true;
                ticket.ClosedDate = DateTime.UtcNow;
                ticket.Status = TicketStatus.Reject;
                ticket.RejectReason = rejectReason;
                ticket.LastModifiedBy = userId;
                ticket.LastModifiedDate = DateTime.UtcNow;

                _ticket.Update(ticket);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<TicketDTO>(ticket);
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> CreateTicket(string token, TicketCreateDTO dto)
        {
            ResultResponse<TicketDTO> response = new()
            ;

            var userId = _tokenService.GetUserId(token);

            var ticketDetails = new List<TicketDetail>();

            foreach (var detail in dto.Details!)
            {
                var item = await _item.GetById(detail.ItemId);

                if (item == null || item.InStock < detail.Quantity)
                {

                    if (item == null)
                    {
                        response.Status = ResponseCode.NotFound;
                        response.Message = new("Item", $"Item #{detail.ItemId} not exists!");
                    }
                    else
                    {
                        response.Status = ResponseCode.UnprocessableContent;
                        response.Message = new("Item", $"Out of stock!");
                    }

                    return response;
                }

                ticketDetails.Add(_mapper.Map<TicketDetail>(detail));
            }

            Ticket ticket = new()
            {
                Purpose = dto.Purpose,
                Title = dto.Title,
                Description = dto.Description,
                Details = ticketDetails,
                PMStatus = TicketPMStatus.Pending,
                Status = TicketStatus.Pending,
                IsClosed = false,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = userId,
                LastModifiedDate = DateTime.UtcNow
            };

            await _ticket.AddAsync(ticket);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<TicketDTO>(ticket);
            response.Status = ResponseCode.Success;
            response.Message = new("Ticket", "Ticket created!");

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TicketDTO>>> GetList(string token)
        {
            ResultResponse<IEnumerable<TicketDTO>> response = new()
            ;

            var userId = _tokenService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            IEnumerable<Ticket>? listTicket;

            if (userRoles.Contains(InventoryRoles.IM))
            {
                listTicket = await _ticket.GetList();
            }
            else if(userRoles.Contains(InventoryRoles.PM))
            {
                listTicket = await _ticket.GetList(user!.TeamId!.Value);
            }
            else
            {
                listTicket = await _ticket.GetList(userId);
            }

            if (listTicket!.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<TicketDTO>>(listTicket);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                //response.Message = new("Ticket", "There is no record!");
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> GetById(string token, Guid id)
        {
            ResultResponse<TicketDTO> response = new();

            var userId = _tokenService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            Ticket? ticket = await _ticket.GetById(id);

            if (!userRoles.Contains(InventoryRoles.IM))
            {
                if (userRoles.Contains(InventoryRoles.PM))
                {
                    if(ticket.CreatedByUser!.TeamId != user!.TeamId)
                    {
                        ticket = null;
                    }
                }
                else
                {
                    if(ticket.CreatedBy != userId)
                    {
                        ticket = null;
                    }
                }
            }

            if(ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<TicketDTO>(ticket);
            }

            return response;    
        }

        public async Task<ResultResponse<TicketDTO>> UpdatePMStatus(string token, Guid ticketId, string? rejectReason = null)
        {
            ResultResponse<TicketDTO> response = new()
            ;

            var userId = _tokenService.GetUserId(token);

            var ticket = await _ticket.GetById(ticketId);
            if(ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                if(rejectReason is null)
                {
                    ticket.PMStatus = TicketPMStatus.Approve;
                    response.Message = new("Ticket", $"You approve for ticket #{ticketId}");
                }
                else
                {
                    ticket.PMStatus = TicketPMStatus.Reject;
                    ticket.RejectReason = rejectReason;
                    ticket.IsClosed = true;
                    ticket.ClosedDate = DateTime.UtcNow;

                    response.Message = new("Ticket", $"You reject ticket #{ticketId}");
                }

                ticket.LastModifiedBy = userId;
                ticket.LastModifiedDate = DateTime.UtcNow;

                _ticket.Update(ticket);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<TicketDTO>(ticket);
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> UpdateStatus(string token, Guid ticketId)
        {
            ResultResponse<TicketDTO> response = new()
            ;

            var userId = _tokenService.GetUserId(token);

            var ticket = await _ticket.GetById(ticketId);

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                if (ticket.PMStatus == TicketPMStatus.Approve)
                {
                    foreach(var detail in ticket.Details!)
                    {
                        var item = await _item.GetById(detail.ItemId);

                        if (item == null || item.InStock < detail.Quantity)
                        {

                            if (item == null)
                            {
                                response.Status = ResponseCode.NotFound;
                                response.Message = new("Item", $"Item #{detail.ItemId} not exists!");
                            }
                            else
                            {
                                response.Status = ResponseCode.UnprocessableContent;
                                response.Message = new("Item", $"Out of stock!");
                            }

                            return response;
                        }
                    }

                    switch (ticket.Status)
                    {
                        case TicketStatus.Pending:
                            ticket.Status++;
                            ticket.LastModifiedBy = userId;
                            ticket.LastModifiedDate = DateTime.UtcNow;

                            response.Status = ResponseCode.Success;
                            response.Message = new("Ticket", "Ticket status updated!");

                            break;

                        case TicketStatus.Processing:
                            ticket.IsClosed = true;
                            ticket.ClosedDate = DateTime.UtcNow;
                            goto case TicketStatus.Pending;

                        case TicketStatus.Done:
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Ticket", "Ticket already done!");
                            break;

                        case TicketStatus.Reject:
                            response.Status = ResponseCode.BadRequest;
                            response.Message = new("Ticket", "Ticket is reject!");
                            break;
                    };

                    _ticket.Update(ticket);
                    await _unitOfWork.SaveAsync();

                    response.Data = _mapper.Map<TicketDTO>(ticket);
                }
                else
                {
                    response.Status = ResponseCode.BadRequest;

                    if(ticket.PMStatus == TicketPMStatus.Reject)
                    {
                        response.Message = new("Ticket", "Ticket is reject by Project Manager!");
                    }

                    response.Message = new("Ticket", "Project Manager still not approve the ticket!");
                }
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> UpdateTicketInfo(string token, Guid ticketId, TicketCreateDTO dto)
        {
            ResultResponse<TicketDTO> response = new()
            ;

            var userId = _tokenService.GetUserId(token);
            
            var ticket = await _ticket.GetById(ticketId);

            var ticketDetails = new List<TicketDetail>();

            if (ticket == null)
            {
                response.Status = ResponseCode.NotFound;
                response.Message = new("Ticket", "Ticket not found!");
            }
            else
            {
                foreach (var detail in dto.Details!)
                {
                    var item = await _item.GetById(detail.ItemId);

                    if (item == null || item.InStock < detail.Quantity)
                    {
                        if (item == null)
                        {
                            response.Status = ResponseCode.NotFound;
                            response.Message = new("Item", $"Item #{detail.ItemId} not exists!");
                        }
                        else
                        {
                            response.Status = ResponseCode.UnprocessableContent;
                            response.Message = new("Item", $"Out of stock!");
                        }

                        return response;
                    }

                    ticketDetails.Add(_mapper.Map<TicketDetail>(detail));
                }

                ticket.Purpose = dto.Purpose;
                ticket.Title = dto.Title;
                ticket.Description = dto.Description;
                ticket.Details = ticketDetails;
                ticket.LastModifiedBy = userId;
                ticket.LastModifiedDate = DateTime.UtcNow;

                _ticket.Update(ticket);
                await _unitOfWork.SaveAsync();

                response.Data = _mapper.Map<TicketDTO>(ticket);
                response.Status = ResponseCode.Success;
                response.Message = new("Ticket", "Ticket Updated!");
            }

            return response;


        }

        public async Task<ResultResponse<IEnumerable<TicketDTO>>> GetMyTickets(string token)
        {
            ResultResponse<IEnumerable<TicketDTO>> response = new()
            ;

            var userId = _tokenService.GetUserId(token);

            var tickets = await _ticket.GetList(userId);

            if (tickets!.Any())
            {
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
                //response.Message = new("Ticket", "There is no record!");
            }

            return response;
        }

        public async Task<PaginationResponse<TicketDTO>> GetPagination(string token, PaginationRequest request)
        {
            PaginationResponse<TicketDTO> response = new();

            var userId = _tokenService.GetUserId(token);
            var user = await _userManager.FindByIdAsync(userId);
            var userRoles = await _userManager.GetRolesAsync(user!);

            var list = await _ticket.GetPagination(request);

            //IEnumerable<Ticket>? listTicket;

            //if (userRoles.Contains(InventoryRoles.IM))
            //{
            //    listTicket = await _ticket.GetList();
            //}
            //else if (userRoles.Contains(InventoryRoles.PM))
            //{
            //    listTicket = await _ticket.GetList(user!.TeamId!.Value);
            //}
            //else
            //{
            //    listTicket = await _ticket.GetList(userId);
            //}

            if (list.Data!.Any())
            {
                response.TotalRecords = list.TotalRecords;
                response.TotalPages = list.TotalPages;
                response.Status = ResponseCode.Success;
                response.Data = _mapper.Map<IEnumerable<TicketDTO>>(list.Data);
            }
            else
            {
                response.Status = ResponseCode.NoContent;
            }

            return response;
        }
    }
}
