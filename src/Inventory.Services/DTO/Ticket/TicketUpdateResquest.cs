using Inventory.Service.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketUpdateResquest
    {
        public int RecordId { get; set; }
        public int TicketTypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public List<TicketEntryUpdateRequest>? TicketEntries { get; set; }
    }
}
