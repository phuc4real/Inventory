using Inventory.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class TicketDetailDTO
    {
        public ItemDTO? Item { get; set; }
        public int Quantity { get; set; }
        public string? Type { get; set; }
    }
    public class TicketDetailCreateDTO
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public TicketDetailType Type { get; set; }
    }
}
