using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Ticket : BaseModel
    {
        public string? TicketName { get; set; }
        public string? Description { get; set; }
        public TicketStatus ProjectManager { get; set; }
        public TicketStatus DepotManager { get; set; }
        public DateTime DoneDate { get; set; }
    }
}
