namespace Inventory.Model.Entity
{
    public class TicketRecord : AuditLog
    {
        public int Id { get; set; }
        public int? TicketId { get; set; }
        public int TicketTypeId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public List<int>? CommentId { get; set; }
    }
}
