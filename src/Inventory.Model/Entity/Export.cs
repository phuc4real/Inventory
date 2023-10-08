namespace Inventory.Model.Entity
{
    public class Export : AuditLog
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public string? ExportFor { get; set; }
    }
}
