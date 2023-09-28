using Inventory.Core.Common;
using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Inventory.Service.Common
{
    public class BaseResponse
    {
        [JsonIgnore]
        public ResponseCode StatusCode { get; set; } = ResponseCode.Success;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResultMessage? Message { get; set; }
    }
}
