using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Order
{
    public class OrderEntryUpdateRequest
    {
        public int ItemId { get; set; }
        public int RecordId { get; set; }
        public int Quantity { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public string? Note { get; set; }
    }
}
