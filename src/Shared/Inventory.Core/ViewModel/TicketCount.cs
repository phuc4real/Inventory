namespace Inventory.Core.ViewModel
{
    public class TicketCount
    {
        public int Pending { get; set; }
        public int Processing { get; set; }
        public int Completed { get; set; }
        public int Rejected { get; set; }
    }
}
