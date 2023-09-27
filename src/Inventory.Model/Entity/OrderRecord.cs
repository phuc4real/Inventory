namespace Inventory.Model.Entity
{
    public class OrderRecord : AuditLog
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int StatusId { get; set; }
        public List<int>? CommentId { get; set; }
        public string? Description { get; set; }
    }
}
