namespace Inventory.Core.ViewModel
{
    public class OrderDetail
    {
        public Item? Item { get; set; }
        public int Quantity { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public string? Note { get; set; }
        public long MinTotal { get; set; }
        public long MaxTotal { get; set; }
    }

    public class UpdateOrderDetail
    {
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public string? Note { get; set; }
        public long MinTotal { get; set; }
        public long MaxTotal { get; set; }
    }
}
