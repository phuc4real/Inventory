using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Request
{
    public class ListItemRequest
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchKeyword { get; set; }
    }
}
