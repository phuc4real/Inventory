using Inventory.Core.Enums;

namespace Inventory.Core.ViewModel
{
    public class Decision
    {
        public string? Status { get; set; }
        public DateTime Date { get; set; }
        public AppUser? ByUser { get; set; }
        public string? Message { get; set; }
    }

    public class UpdateDecision
    {
        public DecisionStatus Status { get; set; }
        public string? Message { get; set; }
    }
}
