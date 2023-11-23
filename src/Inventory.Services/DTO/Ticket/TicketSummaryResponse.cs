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
        public int Review { get; set; }
        public int Pending { get; set; }
        public int Processing { get; set; }
        public int Done { get; set; }
    }

    public class TicketSummaryObjectResponse : ObjectResponse<TicketSummaryResponse> { }
}
