using AutoMapper;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Services.Mapping
{
    public class DTOtoEntity : Profile
    {
        public DTOtoEntity()
        {
            CreateMap<CatalogDTO, CatalogDTO>();
        }
    }
}
