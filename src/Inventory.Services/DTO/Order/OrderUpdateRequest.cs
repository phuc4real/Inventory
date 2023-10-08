using Inventory.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Order
{
    public class OrderUpdateRequest : BaseRequest
    {
        public int RecordId { get; set; }
        public string? Description { get; set; }
        public List<OrderEntryUpdateRequest>? OrderEntries { get; set; }
    }
}
