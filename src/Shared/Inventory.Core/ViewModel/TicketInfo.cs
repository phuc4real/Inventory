using Inventory.Core.Enums;

namespace Inventory.Core.ViewModel
{
    public class TicketInfo
    {
        public int Id { get; set; }
        public string? Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public string? Status { get; set; }
        public Decision? LeaderDecision { get; set; }
        public Decision? Decision { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime CloseAt { get; set; }

        public IList<TicketDetail>? Details { get; set; }
    }

    public class UpdateTicketInfo
    {
        public TicketPurpose Purpose { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public IList<UpdateTicketDetail>? Details { get; set; }
    }

}
