using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class ReceiptDetailDTO
    {
        public ItemDTO? Item { get; set; }
        public int Quantity { get; set; }
    }

    public class ReceiptDetailCreateDTO
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
    }
}
