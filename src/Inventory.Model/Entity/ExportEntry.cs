namespace Inventory.Model.Entity
{
    public class ExportEntry
    {
        public int Id { get; set; }
        public int ExportId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
