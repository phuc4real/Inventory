namespace Inventory.Core.ViewModel
{
    public class ExportDetail
    {
        public Export? Export { get; set; }
        public Item? Item { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
