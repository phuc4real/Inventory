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
        public string? Sender { get; set; }
        public string? SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string? Email { get; set; }
        public string? Key { get; set; }
    }
}
