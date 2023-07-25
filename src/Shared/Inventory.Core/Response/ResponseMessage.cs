using System.Text.Json;

namespace Inventory.Core.Response
{
    public class ResponseMessage
    {
        public ResponseMessage(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string? Key { get; set; }
        public string? Value { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
