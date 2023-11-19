using Inventory.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketSummaryResponse
    {
        public int PendingTicket { get; set; }
        public int ProcessingTicket { get; set; }
        public int RejectTicket { get; set; }
        public int CompletedTicket { get; set; }
    }

    public class TicketSummaryObjectResponse : ObjectResponse<TicketSummaryResponse> { }
}
