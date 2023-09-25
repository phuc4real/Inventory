using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Model.Entity
{
    public class OrderEntry
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public long MinPrice { get; set; }
        public long MaxPrice { get; set; }
        public string? Note { get; set; }
    }
}
