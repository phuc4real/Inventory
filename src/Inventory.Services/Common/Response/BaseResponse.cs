using Inventory.Core.Enums;
using Inventory.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventory.Service.Common.Response
{
    public class BaseResponse
    {
        [JsonIgnore]
        public ResponseCode StatusCode { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResultMessage? Message { get; set; }
    }
}
