using Inventory.Core.Enums;
using System.Text.Json.Serialization;

namespace Inventory.Service.Common
{
    public class PaginationResponse<T> : BaseResponse where T : class
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int PageIndex { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int PageSize { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int TotalPages { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int TotalRecords { get; set; }
        public List<T>? Data { get; set; }
    }
}
