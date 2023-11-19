using Inventory.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Ticket
{
    public class TicketRequest : BaseRequest
    {
        public int TicketId { get; set; }
        public int RecordId { get; set; }
    }
}
