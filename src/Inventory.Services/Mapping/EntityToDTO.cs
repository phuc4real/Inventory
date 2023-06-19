using AutoMapper;
using Inventory.Core.Extensions;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Mapping
{
    public class EntityToDTO : Profile
    {
        public EntityToDTO()
        {
            CreateMap<Catalog, CatalogDTO>();


            CreateMap<Team, TeamDTO>();
            CreateMap<Team, TeamWithMembersDTO>();


            CreateMap<AppUser, AppUserDTO>();

            CreateMap<Item, ItemDTO>();
            CreateMap<Item, ItemDetailDTO>();

            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.Status, opt => opt
                    .MapFrom(src => src.Status
                        .ToDescriptionString())
                 );
            CreateMap<OrderDetail, OrderDetailDTO>();


            CreateMap<Export, ExportDTO>();
            CreateMap<Export, ExportWithDetailDTO>();
            CreateMap<ExportDetail, ExportDetailDTO>();
            CreateMap<ExportDetail, UsingItemDTO>();

            CreateMap<Receipt, ReceiptDTO>();
            CreateMap<ReceiptDetail, ReceiptDetailDTO>();

            CreateMap<Ticket, TicketDTO>()
                .ForMember(dest => dest.Purpose, opt => opt
                    .MapFrom(src => src.Purpose.ToDescriptionString()))
                .ForMember(dest => dest.PMStatus, opt => opt
                    .MapFrom(src => src.PMStatus.ToDescriptionString()))
                .ForMember(dest => dest.Status, opt => opt
                    .MapFrom(src => src.Status.ToDescriptionString()));
            CreateMap<TicketDetail, TicketDetailDTO>()
                .ForMember(dest =>dest.Type, opt => opt
                    .MapFrom(src => src.Type.ToDescriptionString()));

        }
    }
}
