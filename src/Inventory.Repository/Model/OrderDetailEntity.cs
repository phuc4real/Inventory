using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class OrderDetailEntity
    {
        public int OrderInfoId { get; set; }
        public Guid ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public ItemEntity? Item { get; set; }
        public int Quantity { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public string? Note { get; set; }
        public long MinTotal { get; set; }
        public long MaxTotal { get; set; }
    }
}
