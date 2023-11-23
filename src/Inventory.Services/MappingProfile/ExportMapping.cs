using AutoMapper;
using Inventory.Model.Entity;
using Inventory.Service.DTO.Export;

namespace Inventory.Service.MappingProfile
{
    public class ExportMapping : Profile
    {
        public ExportMapping()
        {
            CreateMap<Export, ExportResponse>();
            CreateMap<ExportEntry, ExportEntryResponse>();

            CreateMap<TicketEntry, ExportEntry>()
                .ForMember(x => x.Id, y => y.Ignore());

        }
    }
}
