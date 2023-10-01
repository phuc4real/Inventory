using Inventory.Core.Enums;
using System.Text.Json.Serialization;

namespace Inventory.Service.Common
{
    public class PaginationResponse<T> : BaseResponse where T : class
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int Page { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int Count { get; set; }
        public List<T>? Data { get; set; }
    }
}
