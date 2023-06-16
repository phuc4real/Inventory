using AutoMapper;
using Azure;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;

namespace Inventory.Services.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticket;
        private readonly IItemRepository _item;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public TicketService(
            ITicketRepository ticket,
            IItemRepository item,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITokenService tokenService)
        {
            _ticket = ticket;
            _item = item;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<ResultResponse<TicketDTO>> RejectTicket(string token, Guid ticketId, string rejectReason)
        {
            ResultResponse<TicketDTO> response = new()
            { Messages = new List<ResponseMessage>() { } };

            var userId = _tokenService.GetUserId(token);

            var ticket = await _ticket.GetById(ticketId);

            if (ticket == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "Ticket not found!"));
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

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<TicketDTO>(ticket);
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> CreateTicket(string token, TicketCreateDTO dto)
        {
            ResultResponse<TicketDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            var ticketDetails = new List<TicketDetail>();

            foreach (var detail in dto.Details!)
            {
                var itemExists = await _item.AnyAsync(x => x.Id == detail.ItemId);

                if (!itemExists)
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Item", $"Item #{detail.ItemId} not exists!"));

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
                CreatedDate = DateTime.Now,
                LastModifiedBy = userId,
                LastModifiedDate = DateTime.Now
            };

            await _ticket.AddAsync(ticket);
            await _unitOfWork.SaveAsync();

            response.Data = _mapper.Map<TicketDTO>(ticket);
            response.Status = ResponseStatus.STATUS_SUCCESS;
            response.Messages.Add(new ResponseMessage("Ticket", "Ticket created!"));

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TicketDTO>>> GetAll()
        {
            ResultResponse<IEnumerable<TicketDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var tickets = await _ticket.GetAllAsync();

            if (tickets.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "There is no record!"));
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> GetTicketById(Guid id)
        {
            ResultResponse<TicketDTO> response = new()
            {
                Messages = new List<ResponseMessage>()
            };

            var ticket = await _ticket.GetById(id);

            if(ticket == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "Ticket not found!"));
            }
            else
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<TicketDTO>(ticket);
            }

            return response;    
        }

        public async Task<ResultResponse<TicketDTO>> PMStatus(string token, Guid ticketId, string? rejectReason = null)
        {
            ResultResponse<TicketDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            var ticket = await _ticket.GetById(ticketId);
            if(ticket == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "Ticket not found!"));
            }
            else
            {
                if(rejectReason is null)
                {
                    ticket.PMStatus = TicketPMStatus.Approve;
                    response.Messages.Add(new ResponseMessage("Ticket", $"You approve for ticket #{ticketId}"));
                }
                else
                {
                    ticket.PMStatus = TicketPMStatus.Reject;
                    ticket.RejectReason = rejectReason;
                    ticket.IsClosed = true;
                    ticket.ClosedDate = DateTime.UtcNow;

                    response.Messages.Add(new ResponseMessage("Ticket", $"You reject ticket #{ticketId}"));
                }

                ticket.LastModifiedBy = userId;
                ticket.LastModifiedDate = DateTime.UtcNow;

                _ticket.Update(ticket);
                await _unitOfWork.SaveAsync();

                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<TicketDTO>(ticket);
            }

            return response;
        }

        public async Task<ResultResponse<IEnumerable<TicketDTO>>> TicketsByItemId(Guid itemId)
        {
            ResultResponse<IEnumerable<TicketDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var item = await _item.GetById(itemId);

            if(item == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Item", $"Item #{itemId} not found!"));
            }
            else
            {
                var tickets = await _ticket.TicketsByItem(item);

                if (tickets.Any())
                {
                    response.Status = ResponseStatus.STATUS_SUCCESS;
                    response.Data = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
                }
                else
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;
                    response.Messages.Add(new ResponseMessage("Ticket", "There is no record!"));
                }
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> UpdateStatus(string token, Guid ticketId)
        {
            ResultResponse<TicketDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);

            var ticket = await _ticket.GetById(ticketId);

            if (ticket == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "Ticket not found!"));
            }
            else
            {
                if (ticket.PMStatus == TicketPMStatus.Approve)
                {
                    switch (ticket.Status)
                    {
                        case TicketStatus.Pending:
                            ticket.Status++;
                            ticket.LastModifiedBy = userId;
                            ticket.LastModifiedDate = DateTime.UtcNow;

                            response.Status = ResponseStatus.STATUS_SUCCESS;
                            response.Messages.Add(new ResponseMessage("Ticket", "Ticket status updated!"));

                            break;

                        case TicketStatus.Processing:
                            ticket.IsClosed = true;
                            ticket.ClosedDate = DateTime.UtcNow;
                             goto case TicketStatus.Pending;

                        case TicketStatus.Done:
                            response.Status = ResponseStatus.STATUS_FAILURE;
                            response.Messages.Add(new ResponseMessage("Ticket", "Ticket already done!"));
                            break;

                        case TicketStatus.Reject:
                            response.Status = ResponseStatus.STATUS_FAILURE;
                            response.Messages.Add(new ResponseMessage("Ticket", "Ticket is reject!"));
                            break;
                    };

                    _ticket.Update(ticket);
                    await _unitOfWork.SaveAsync();

                    response.Data = _mapper.Map<TicketDTO>(ticket);
                }
                else
                {
                    response.Status = ResponseStatus.STATUS_FAILURE;

                    if(ticket.PMStatus == TicketPMStatus.Reject)
                        response.Messages.Add(new ResponseMessage("Ticket", "Ticket is reject by Project Manager!"));
                    response.Messages.Add(new ResponseMessage("Ticket", "Project Manager still not approve the ticket!"));
                }
            }

            return response;
        }

        public async Task<ResultResponse<TicketDTO>> UpdateTicketInfo(string token, Guid ticketId, TicketCreateDTO dto)
        {
            ResultResponse<TicketDTO> response = new()
            { Messages = new List<ResponseMessage>() };

            var userId = _tokenService.GetUserId(token);
            
            var ticket = await _ticket.GetById(ticketId);

            var ticketDetails = new List<TicketDetail>();

            if (ticket == null)
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "Ticket not found!"));
            }
            else
            {
               

                foreach (var detail in dto.Details!)
                {
                    var itemExists = await _item.AnyAsync(x => x.Id == detail.ItemId);

                    if (!itemExists)
                    {
                        response.Status = ResponseStatus.STATUS_FAILURE;
                        response.Messages.Add(new ResponseMessage("Item", $"Item #{detail.ItemId} not exists!"));

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
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Messages.Add(new ResponseMessage("Ticket", "Ticket Updated!"));
            }

            return response;


        }

        public async Task<ResultResponse<IEnumerable<TicketDTO>>> SearchTicket(string filter)
        {
            ResultResponse<IEnumerable<TicketDTO>> response = new()
            { Messages = new List<ResponseMessage>() };

            var tickets = await _ticket.GetWithFilter(filter);

            if (tickets.Any())
            {
                response.Status = ResponseStatus.STATUS_SUCCESS;
                response.Data = _mapper.Map<IEnumerable<TicketDTO>>(tickets);
            }
            else
            {
                response.Status = ResponseStatus.STATUS_FAILURE;
                response.Messages.Add(new ResponseMessage("Ticket", "There is no record!"));
            }

            return response;
        }
    }
}
