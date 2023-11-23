namespace Inventory.Model.Entity
{
    public class Order : AuditLog
    {
        public int Id { get; set; }
        public DateTime? CompleteDate { get; set; }
    }
}
