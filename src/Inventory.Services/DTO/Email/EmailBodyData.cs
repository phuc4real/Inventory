namespace Inventory.Service.DTO.Email
{
    public class EmailBodyData
    {
        public bool IsTicket { get; set; } = false;
        public int InfoId { get; set; }
        public int RecordId { get; set; }
        public string? TicketType { get; set; }
        public string? Title { get; set; }
        public DateTime InfoCreatedAt { get; set; }
        public string? InfoCreatedBy { get; set; }
        public string? Description { get; set; }
    }
}
