using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        public int TicketNumber { get; set; }
        public string? Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool PMApprove { get; set; }
        public TicketStatus Status { get; set; }
        public string? RejectReason { get; set; }
        public DateTime ClosedDate { get; set; }

        public IList<Item>? Items { get; set; }
        public IList<TicketDetail>? Details { get; set; }
    }
}
