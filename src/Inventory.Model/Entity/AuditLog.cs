namespace Inventory.Model.Entity
{
    public class AuditLog
    {
        //Create log
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        //Update log
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        //Deactive log
        public bool IsInactive { get; set; }
        public DateTime? InactiveAt { get; set; }
        public string? InactiveBy { get; set; }
    }
}
