using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Response
{
    public class BaseResponse
    {
        public ResponeStatus Status { get; set; }
        public IList<ResponseMessage>? Messages { get; set; }
    }
}
