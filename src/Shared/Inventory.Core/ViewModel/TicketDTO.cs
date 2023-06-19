using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class TicketDTO
    {
        public Guid Id { get; set; }
        public string? Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? PMStatus { get; set; }
        public string? Status { get; set; }
        public string? RejectReason { get; set; }

        public bool IsClosed { get; set; }
        public DateTime ClosedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public AppUserDTO? CreatedByUser { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public AppUserDTO? ModifiedByUser { get; set; }

        public IList<TicketDetailDTO>? Details { get; set; }
    }

    public class TicketCreateDTO
    {
        public TicketPurpose Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public IList<TicketDetailCreateDTO>? Details { get; set; }
    }
}
