namespace Inventory.Model.Entity
{
    public class TicketEntry
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
