using System.Text.Json.Serialization;

namespace Inventory.Core.Response
{
    public class ResultResponse<T> where T : class
    {
        [JsonIgnore]
        public string? Status { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IList<ResponseMessage>? Messages { get; set; }
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
    }
}
