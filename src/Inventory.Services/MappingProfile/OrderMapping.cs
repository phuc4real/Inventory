using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class OrderMapping : Profile
    {
        public OrderMapping()
        {
            CreateMap<Order,OrderResponse>();
            CreateMap<OrderResponse, OrderResponse>();
            CreateMap<OrderEntryUpdateRequest, OrderEntry>();
        }

    }
}
