using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Model
{
    public class ExportDetail
    {
        public int ExportId { get; set; }
        [ForeignKey(nameof(ExportId))]
        public Export? Export { get; set; }
        public Guid ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public Item? Item { get; set; }
        public int Quantity { get; set; }
        public string? ForUserId { get; set; }
        [ForeignKey(nameof(ForUserId))]
        public AppUser? ForUser { get; set; }
    }
}
