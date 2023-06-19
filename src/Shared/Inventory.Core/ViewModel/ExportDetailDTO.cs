using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.ViewModel
{
    public class ExportDetailDTO
    {
        public ItemDTO? Item { get; set; }
        public int Quantity { get; set; }
        public AppUserDTO? ForUser { get; set; }
    }
    public class ExportDetailCreateDTO
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public string? ForUserId { get; set; }
    }
}
