using System.Text.Json;

namespace Inventory.Core.Common
{
    public class ResultMessage
    {
        public ResultMessage() { }

        public ResultMessage(string key, string message)
        {
            Key = key;
            Message = message;
        }

        public string? Key { get; set; }
        public string? Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
