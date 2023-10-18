using Inventory.Service.Common;
using Inventory.Service.DTO.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Export
{
    public class ExportEntryResponse
    {
        public int Id { get; set; }
        public int ExportId { get; set; }
        public ItemCompactResponse? Item { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

    public class ExportEntryListResponse : PaginationResponse<ExportEntryResponse> { }
}
