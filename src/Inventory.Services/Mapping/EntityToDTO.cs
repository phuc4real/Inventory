using AutoMapper;
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
        }
    }
}
