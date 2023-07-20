using Inventory.Core.Enums;
using Inventory.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class Ticket
    {
        [Key]
        public Guid Id { get; set; }
        public TicketPurpose Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public LeaderApprove LeaderApprove { get; set; }
        public TicketStatus Status { get; set; }
        public string? RejectReason { get; set; }

        public bool IsClosed { get; set; }
        public DateTime ClosedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        [ForeignKey(nameof(CreatedBy))]
        public AppUser? CreatedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        [ForeignKey(nameof(LastModifiedBy))]
        public AppUser? ModifiedByUser { get; set; }

        public IList<Item>? Items { get; set; }
        public IList<TicketDetail>? Details { get; set; }
    }
}
