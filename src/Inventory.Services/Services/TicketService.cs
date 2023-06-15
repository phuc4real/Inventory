using AutoMapper;
using Azure;
using Inventory.Core.Common;
using Inventory.Core.Enums;
using Inventory.Core.Response;
using Inventory.Core.ViewModel;
using Inventory.Repository.IRepository;
using Inventory.Repository.Model;
using Inventory.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task<ResultResponse<TicketDTO>> CancelTicket(string token, Guid ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultResponse<TicketDTO>> CloseTicket(string token, TicketCancelDTO dto)
        {
            throw new NotImplementedException();
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
                PMApprove = TicketStatus.Pending,
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

        public Task<ResultResponse<TicketDTO>> PMApprove(string token, Guid ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultResponse<IEnumerable<TicketDTO>>> TicketsByItemId()
        {
            throw new NotImplementedException();
        }

        public Task<ResultResponse<TicketDTO>> UpdateStatus(string token, Guid ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultResponse<TicketDTO>> UpdateTicketInfo(string token, TicketCreateDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
