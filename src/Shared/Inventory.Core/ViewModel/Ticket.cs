namespace Inventory.Core.ViewModel
{
    public class Ticket
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public AppUser? CreatedByUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public AppUser? UpdatedByUser { get; set; }

        public DateTime? CloseDate { get; set; }
    }

    public class TicketWithHistory : Ticket
    {
        public IList<TicketInfo>? History { get; set; }
    }
}
