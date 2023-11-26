using Inventory.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.Common
{
    public class StatusIdCollections
    {
        public int ReviewId { get; set; }
        public int PendingId { get; set; }
        public int ProcessingId { get; set; }
        public int CancelId { get; set; }
        public int DoneId { get; set; }
        public int RejectId { get; set; }
        public int CloseId { get; set; }

        public List<int>? CanEdit => new() { ReviewId, PendingId, ProcessingId, RejectId };
        public List<int>? CanCancelTicket => new() { ReviewId, PendingId, RejectId };
        public List<int>? CannotEdit => new() { CancelId, DoneId, CloseId };
        public List<int>? SummaryId => new() { ReviewId, PendingId, ProcessingId, DoneId };
        public List<Status>? Data { get; set; }
    }
}
