using Inventory.Core.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class TicketDetailEntity
    {
        public int TicketInfoId { get; set; }
        public Guid ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public ItemEntity? Item { get; set; }
        public int Quantity { get; set; }
        public TicketDetailType Type { get; set; }
        public string? Note { get; set; }
    }
}
