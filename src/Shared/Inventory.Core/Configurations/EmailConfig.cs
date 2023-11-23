using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Configurations
{
    public class EmailConfig
    {
        private const string _name = "Email";
        public static string Name => _name;
        public string? ApiKey { get; set; }
        public string? Url { get; set; }
        public string? Domain { get; set; }
        public string? From { get; set; }
    }
}
