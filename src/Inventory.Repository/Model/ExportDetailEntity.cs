using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Repository.Model
{
    public class ExportDetailEntity
    {
        public int ExportId { get; set; }
        [ForeignKey(nameof(ExportId))]
        public ExportEntity? Export { get; set; }
        public Guid ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]
        public ItemEntity? Item { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
