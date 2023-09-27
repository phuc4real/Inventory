using AutoMapper;
using Inventory.Core.ViewModel;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Export;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.MappingProfile
{
    public class ExportMapping : Profile
    {
        public ExportMapping()
        {
            CreateMap<Export, ExportResponse>();
            CreateMap<ExportEntry, ExportEntryResponse>();
        }
    }
}
