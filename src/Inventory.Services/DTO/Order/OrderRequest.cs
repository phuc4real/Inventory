using Inventory.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Order
{
    public class OrderRequest : BaseRequest
    {
        public int RecordId { get; set; }
    }
}
