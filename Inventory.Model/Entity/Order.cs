using System.ComponentModel.DataAnnotations;

namespace Inventory.Model.Entity
{
    public class Order : AuditLog
    {
        public int Id { get; set; }
        public DateTime? CompleteDate { get; set; }
        public List<OrderRecord>? Records { get; set; }
    }
}
