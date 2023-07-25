namespace Inventory.Core.ViewModel
{
    public class InUse
    {
        public int ExportId { get; set; }

        public Guid ItemId { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }

        public int Quantity { get; set; }
        public AppUser? ForUser { get; set; }
    }
}
