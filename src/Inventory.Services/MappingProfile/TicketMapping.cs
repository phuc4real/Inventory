using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Ticket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class TicketMapping : Profile
    {
        public TicketMapping()
        {
            CreateMap<Ticket, TicketResponse>();
            CreateMap<TicketResponse, TicketResponse>();
            CreateMap<TicketType, TicketTypeResponse>();
        }
    }
}
