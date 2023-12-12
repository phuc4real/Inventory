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
        public ResultMessage? Message { get; private set; }

        public void AddMessage(string message)
        {
            Message = new ResultMessage("Success", message);
        }

        public void AddMessage(ResultMessage message)
        {
            Message = message;
        }

        public void AddError(string message)
        {
            StatusCode = ResponseCode.BadRequest;
            Message = new ResultMessage("Error", message);
        }

        public void AddError(ResultMessage message)
        {
            StatusCode = ResponseCode.BadRequest;
            Message = message;
        }
    }
}
