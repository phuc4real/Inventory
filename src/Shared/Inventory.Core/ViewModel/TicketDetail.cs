using Inventory.Core.Enums;

namespace Inventory.Core.ViewModel
{
    public class TicketDetail
    {
        public int TicketInfoId { get; set; }
        public Item? Item { get; set; }
        public int Quantity { get; set; }
        public string? Type { get; set; }
        public string? Note { get; set; }
    }
    public class UpdateTicketDetail
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public TicketDetailType Type { get; set; }
        public string? Note { get; set; }
    }
}
