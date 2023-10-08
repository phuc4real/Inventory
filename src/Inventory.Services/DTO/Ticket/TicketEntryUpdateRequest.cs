using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketEntryUpdateRequest
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
