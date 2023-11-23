using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class ItemMapping : Profile
    {
        public ItemMapping()
        {
            CreateMap<Item, ItemResponse>();
            CreateMap<Item, ItemCompactResponse>();
            CreateMap<ItemUpdateRequest, Item>();
        }
    }
}
