namespace Inventory.Core.ViewModel
{
    public class Order
    {
        public int Id { get; set; }
        public string? Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public AppUsers? CreatedByUser { get; set; }
        public DateTime UpdatedDate { get; set; }
        public AppUsers? UpdatedByUser { get; set; }

        public DateTime? CompleteDate { get; set; }
    }

    public class OrderWithHistory : Order
    {
        public IList<OrderInfo>? History { get; set; }
    }
}
