using Inventory.Core.Enums;
using System.Text.Json.Serialization;

namespace Inventory.Core.Response
{
    public class ResultResponse<T> where T : class
    {
        [JsonIgnore]
        public ResponseCode Status { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ResponseMessage? Message { get; set; }
    }
}
