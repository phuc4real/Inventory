using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class UsingItemDTO
    {
        public ExportDTO? Export { get; set; }
        public ItemDTO? Item { get; set; }
        public int Quantity { get; set; }
        public AppUserDTO? ForUser { get; set; }
    }
}
