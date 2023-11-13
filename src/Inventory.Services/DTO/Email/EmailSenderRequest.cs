using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Service.DTO.Email
{
    public class EmailSenderRequest
    {
        public string? SendTo { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}
