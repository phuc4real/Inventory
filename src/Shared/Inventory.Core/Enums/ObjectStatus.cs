namespace Inventory.Core.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Done = 3,
        Cancel = 4
    }

    public enum ExportStatus
    {
        Pending = 1,
        Processing = 2,
        Done = 3,
    }

    public enum TicketStatus
    {
        Pending = 1,
        Processing = 2,
        Done = 3,
        Reject = 4,
        Close = 5
    }

    public enum DecisionStatus
    {
        Pending = 1,
        Approve = 2,
        Reject = 3
    }
}
