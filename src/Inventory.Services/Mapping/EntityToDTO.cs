using AutoMapper;
using Inventory.Core.Extensions;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Services.Mapping
{
    public class EntityToDTO : Profile
    {
        public EntityToDTO()
        {
            CreateMap<CatalogEntity, Catalog>();

            CreateMap<AppUserEntity, AppUser>();

            CreateMap<ItemEntity, Item>();
            CreateMap<ItemEntity, ItemDetail>();

            CreateMap<DecisionEntity, Decision>()
                .ForMember(dest => dest.Status, opt => opt
                    .MapFrom(src => src.Status
                        .ToDescriptionString()));

            CreateMap<OrderEntity, Order>();
            CreateMap<OrderEntity, OrderWithHistory>();
            CreateMap<OrderInfoEntity, OrderInfo>()
                .ForMember(dest => dest.Status, opt => opt
                    .MapFrom(src => src.Status
                        .ToDescriptionString()));

            CreateMap<OrderDetailEntity, OrderDetail>();

            CreateMap<ExportEntity, Export>();
            CreateMap<ExportDetailEntity, ExportDetail>();
            CreateMap<ExportDetailEntity, InUse>();

            CreateMap<TicketEntity, Ticket>();
            CreateMap<TicketEntity, TicketWithHistory>();
            CreateMap<TicketInfoEntity, TicketInfo>()
                .ForMember(dest => dest.Purpose, opt => opt
                    .MapFrom(src => src.Purpose.ToDescriptionString()))
                .ForMember(dest => dest.Status, opt => opt
                    .MapFrom(src => src.Status.ToDescriptionString()));

            CreateMap<TicketDetailEntity, TicketDetail>()
                .ForMember(dest => dest.Type, opt => opt
                    .MapFrom(src => src.Type.ToDescriptionString()));

        }
    }
}
