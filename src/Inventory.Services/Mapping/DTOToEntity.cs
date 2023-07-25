using AutoMapper;
using Inventory.Core.ViewModel;
using Inventory.Repository.Model;

namespace Inventory.Services.Mapping
{
    public class DTOtoEntity : Profile
    {
        public DTOtoEntity()
        {
            CreateMap<Catalog, CatalogEntity>();
            CreateMap<UpdateCatalog, CatalogEntity>();

            CreateMap<AppUser, AppUserEntity>();

            CreateMap<Item, ItemEntity>();
            CreateMap<ItemDetail, ItemEntity>();
            CreateMap<UpdateItem, ItemEntity>();

            CreateMap<UpdateOrderInfo, OrderInfoEntity>();
            CreateMap<UpdateOrderDetail, OrderDetailEntity>();

            CreateMap<UpdateTicketInfo, TicketInfoEntity>();
            CreateMap<UpdateTicketDetail, TicketDetailEntity>();
        }
    }
}
