using Inventory.Service.Common;
using Inventory.Service.DTO.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketEntryResponse
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public ItemResponse? Item { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

    public class TicketEntryList : PaginationResponse<TicketEntryResponse> { }
}
