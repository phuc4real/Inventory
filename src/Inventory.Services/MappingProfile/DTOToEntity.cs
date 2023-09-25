using AutoMapper;
using Inventory.Core.ViewModel;
using Inventory.Model.Entity;

namespace Inventory.Service.MappingProfile
{
    public class DTOtoEntity : Profile
    {
        public DTOtoEntity()
        {



            CreateMap<Item, Item>();
            CreateMap<ItemDetail, Model.Entity.Item>();
            CreateMap<UpdateItem, Model.Entity.Item>();

            CreateMap<UpdateOrderInfo, Model.Entity.OrderRecord>();
            CreateMap<UpdateOrderDetail, OrderEntry>();

            CreateMap<UpdateTicketInfo, TicketRecord>();
            CreateMap<UpdateTicketDetail, TicketEntry>();
        }
    }
}
