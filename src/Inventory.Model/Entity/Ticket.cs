namespace Inventory.Model.Entity
{
    public class Ticket : AuditLog
    {
        public int Id { get; set; }
        public DateTime? CloseDate { get; set; }
    }
}
